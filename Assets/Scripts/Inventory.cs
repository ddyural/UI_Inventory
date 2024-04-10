using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Inventory : MonoBehaviour
{
    public DataBase data; // номер предмета, его количество, картинка - всё будет считываться и запоминаться
    public List<ItemInventory> items = new List<ItemInventory>();
    public GameObject _gameObjShow; // какие-то геймОбжекты должны быть отражены
    // а некоторые просто стакаться и отправляться как доп подпункты и исчезать
    public GameObject InventoryMainObject; // надо подключить то, за что этот блок будет отвечать
    public int _maxCount;

    public Camera _cam;

    public EventSystem _es;

    public int _currentID;
    public ItemInventory _currentItem;

    public RectTransform _movingObject;
    public Vector3 _offest; // когда мы будем брать какой-то элемент, он должен немного сместиться от курсора

    public GameObject _backGround; // фон

    public RectTransform _inventoryMainRectTransform;
    
    // Поле для хранения ссылок на компоненты TextMeshProUGUI и Image
    private Dictionary<int, TextMeshProUGUI[]> _textComponents = new Dictionary<int, TextMeshProUGUI[]>();
    private Dictionary<int, Image[]> _imageComponents = new Dictionary<int, Image[]>();

    /// <summary>
    /// заполняем ячейки
    /// </summary>
    public void Start()
    {
        _inventoryMainRectTransform = InventoryMainObject.GetComponent<RectTransform>();
        
        if (items.Count == 0)
        {
            AddGraphics();
        }

        for (int i = 0; i < _maxCount; i++) // заполнение ячеек рандомно элементами, просто тест
        {
            AddItem(i, data.items[Random.Range(0, data.items.Count)], Random.Range(1, 99));
        }
    }

    public void Update()
    {
        if (_currentID != -1 )
        {
            MoveObject();
        }
        UpdateInventory();

        if (Input.GetKeyDown(KeyCode.I)) // открываем и закрываем инвентарь 
        {
            _backGround.SetActive(!_backGround.activeSelf); // активселф будет открывать и закрывать
            if (_backGround.activeSelf)
            {
                UpdateInventory();
            }

        }
    }

    /// <summary>
    /// позволяет стакаться предметам
    /// </summary>
    /// <param name="item">вещь</param>
    /// <param name="_count">количество</param>
    public void SearchForSameItem(Item item, int _count)
    {
        for(int i = 0; i < _maxCount; i++)
        {
            if (items[i]._id == item._id) // это 1 и тот же предмет
            {
                if (items[0]._count < 128) // стак - 128 предметов в ячейке
                {
                    items[i]._count += _count; // суммирует кол-во предметов в ячейке 

                    if (items[i]._count > 128) // ячейка максимально заполнена
                    {
                        _count = items[i]._count - 128; // остаётся столько, сколько не поместилось в стак
                        items[i]._count = 64;
                    }
                    else
                    {
                        _count = 0;
                        i = _maxCount;
                    }
                }
            }
        }

        if (_count > 0)
        {
            for(int i = 0; i < _maxCount; i++)
            {
                if (items[i]._id == 0)
                {
                    AddItem(i, item, _count);
                    i = _maxCount;
                    // если ячейка пустая, то в неё поместится предмет, который мы подобрали
                    // или который не уместился в стак
                    // в пустые ячейки может что-то помещаться

                    // ячейки надо будет проверить и поместить 1 непоместившийся объектт
                }
            }
        }
    }


    /// <summary>
    /// добвить итем
    /// </summary>
    /// <param name="_id">для сортировки</param>
    /// <param name="item">сам предмет</param>
    /// <param name="_count">количество в стаке</param>
    public void AddItem(int _id, Item item, int _count)
    {
        items[_id]._id = item._id; // наш id айтемсов будет считываться, сравниваться, переписываться, поэтому нужно
        items[_id]._count = _count;
        items[_id]._itemGameObject.GetComponent<Image>().sprite = item._image; // data.items[Item._id];
        // спрайт фоточка. Чтоб он забирал инфу с скрипта нашего DataBase и отображал здесь изображение
        // + чтобы считывалось id из инвентаря 

        TextMeshProUGUI[] texts = items[_id]._itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            text.text = (_count > 0 && item._id != 0) ? _count.ToString() : "";
            // если в ячейке объектов больше одного, то будет забирать инфу 
            // у подчинённого предмета и переведёт количество в текст
            // это позволит сплюсовать те элементы, которые у нас есть
        }
    }
        
    /// <summary>
    /// добавить инвентарь-итем
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="invItem"></param>
    public void AddInventoryItem(int _id, ItemInventory invItem)
    {
        items[_id]._id = invItem._id;
        items[_id]._count = invItem._count;
        items[_id]._itemGameObject.GetComponent<Image>().sprite = data.items[invItem._id]._image; // = item._image; 
        // спрайт фоточка. Чтоб он забирал инфу с скрипта нашего DataBase и отображал здесь изображение
        // + чтобы считывалось id из инвентаря 

        TextMeshProUGUI[] texts = items[_id]._itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            text.text = (invItem._count > 0 && invItem._id != 0) ? invItem._count.ToString() : "";
            // если в ячейке объектов больше одного, то будет забирать инфу 
            // у подчинённого предмета и переведёт количество в текст
            // это позволит сплюсовать те элементы, которые у нас есть
        }
    }

    /// <summary>
    /// будет обновляться и считывать, какая картинка находится в ячейке и какие меняющиеся числа 
    /// </summary>
    public void AddGraphics()
    {
        for (int i = 0; i < _maxCount; i++)
        {
            GameObject newItem = Instantiate(_gameObjShow, InventoryMainObject.transform);
            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory();
            ii._itemGameObject = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero; 
            rt.localScale = Vector3.one; // чтобы ячейки не скакали и были одинаковые

            // получаем все компоненты RectTransform в дочерних объектах newItem (нового объекта)
            // устанавливает для каждого из них масштаб scale равным (1, 1, 1)
            RectTransform[] childRTs = newItem.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform childRT in childRTs)
            {
                childRT.localScale = Vector3.one;
            }
            // чтоб когда мы добавляем элементы и берём 
            // этот элемент, чтобы scale был такой же



            Button tempButton = newItem.GetComponent<Button>(); // каждый пункт инвенторя - кнопка
            // но можем нажимать только когда там что-то есть

            tempButton.onClick.AddListener(delegate { SelectObject(); });
            // обработчик события клика на кнопке tempButton
            // когда пользователь кликает на эту кнопку, будет вызван метод SelectObject().

            items.Add(ii); // добавляем элемент в ItemInventory
        }
    }


    /// <summary>
    /// передвигаем объект
    /// </summary>
    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + _offest; // то есть когда мы передвигаем объект, картинка чуть-чуть смещаться
        pos.z = _inventoryMainRectTransform.position.z;
        _movingObject.position = _cam.ScreenToWorldPoint(pos); // изображения будут отталкиваться от камеры, где мы хватаем элемент и где у нас на камере
        // где мы схватили и поместили
    }

    // Метод для обновления ссылок на компоненты TextMeshProUGUI и Image
    public void UpdateItemComponents(int itemId)
    {
        // Получаем объект элемента по ID
        GameObject itemGameObject = items[itemId]._itemGameObject;

        // Получаем компоненты TextMeshProUGUI и Image для данного элемента
        TextMeshProUGUI[] texts = itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = itemGameObject.GetComponentsInChildren<Image>();

        // Сохраняем ссылки в словари
        _textComponents[itemId] = texts;
        _imageComponents[itemId] = images;
    }

    /// <summary>
    ///  выполняет обновление отображения инвентаря
    /// </summary>
    public void UpdateInventory()
    {
        for (int i = 0; i < _maxCount; i++)
        {
            if (_textComponents.ContainsKey(i) && _imageComponents.ContainsKey(i))
            {
                if (items[i]._id != 0 && items[i]._count > 1)
                {
                    foreach (TextMeshProUGUI text in _textComponents[i])
                    {
                        text.text = items[i]._count.ToString();
                    }
                }
                else
                {
                    foreach (TextMeshProUGUI text in _textComponents[i])
                    {
                        text.text = "";
                    }
                }

                foreach (Image image in _imageComponents[i])
                {
                    image.sprite = data.items[items[i]._id]._image;
                }
            }
        }
    }


    /// <summary>
    /// обработка выбора и перемещения элементов в инвентаре
    /// </summary>
    public void SelectObject()
    {
        int _selectedId = int.Parse(_es.currentSelectedGameObject.name);
        ItemInventory _selectedSlot = items[_selectedId];

        if (_selectedSlot._id == 0)
        {
            // необходимо игнорировать пустые слоты при выборе для перемещения
            Debug.Log("You can't select an empty slot to move.");
            
            return;
        }
        
        if (_currentID == -1) // пустая ячейка
        {
            if (_selectedSlot._id == 0)
            {
                // необходимо игнорировать пустые слоты при выборе для перемещения
                Debug.Log("You can't select an empty slot to move.");
                
            }
            else
            {
                _currentID = int.Parse(_es.currentSelectedGameObject.name); // в число
                _currentItem = CopyInventoryItem(items[_currentID]); // создание копии
                _movingObject.gameObject.SetActive(true);
                _movingObject.GetComponent<Image>().sprite = data.items[_currentItem._id]._image; 
                // задаём спрайт перемещаемому объекту на основе данных элемента

                AddItem(_currentID, data.items[0], 0); // id, item, count
                // добавляем пустой элемент в инвентарь для замены перемещаемого элемента
            }
        }
        else
        {
            ItemInventory II = items[int.Parse(_es.currentSelectedGameObject.name)];

            if (_currentItem._id != II._id)
            {
                AddInventoryItem(_currentID, II);
                // добавляем выбранный элемент в инвентарь

                AddInventoryItem(int.Parse(_es.currentSelectedGameObject.name), _currentItem);
                // замена выбранного элемента на перемещаемый элемент
            }
            else
            {
                if (II._count + _currentItem._count <= 128) // будет стакаться до 128
                {
                    II._count += _currentItem._count;
                }
                else
                {
                    // в ячейке останется 128, а остаток улетит в другую ячейку
                    AddItem(_currentID, data.items[II._id], II._count + _currentItem._count - 128); 

                    II._count = 128;
                }

                II._itemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = II._count.ToString();

            }
            
            _currentID = -1;

            _movingObject.gameObject.SetActive(false); // всё, мы перенесли наш предмет
        }
    }

    /// <summary>
    /// когда мы берём курсором что-то, то всё содержимое ячейки должно скопироваться в момент нашего переноса
    /// </summary>
    /// <param name="old">наша вещь</param>
    /// <returns></returns>
    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory New = new ItemInventory();

        New._id = old._id; // когда мы куда-то ставим наш предмет в инвентаре, то он должен считать старую инфу и поместить в новую
        New._itemGameObject = old._itemGameObject;
        New._count = old._count; // и кол-во

        return New;
    }
}

/// <summary>
/// расписан объект
/// </summary>
[System.Serializable]
public class ItemInventory
{
    public int _id;
    public GameObject _itemGameObject; // тут мы можем поместить GameObj, который будем юзать
    public int _count;  // показывать, сколько в стаке элементов
}