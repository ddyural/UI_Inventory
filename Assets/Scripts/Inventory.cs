using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    /// <summary>
    /// добавление предметика
    /// </summary>
    /// <param name="_id">для сортировки</param>
    /// <param name="item">сам предмет</param>
    /// <param name="_count">количество в стаке</param>
    public void AddItem(int _id, Item item, int _count)
    {
        items[_id]._id = item._id; // наш id айтемсов будет считываться, сравниваться, переписываться, поэтому нужно
        items[_id]._count = _count;
        items[_id]._itemGameObject.GetComponent<Image>().sprite = item._image;  // = item._image; 
        // спрайт фоточка. Чтоб он забирал инфу с скрипта нашего DataBase и отображал здесь изображение
        // + чтобы считывалось id из инвентаря 

        Text[] texts = items[_id]._itemGameObject.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            text.text = (_count > 0 && item._id != 0) ? _count.ToString() : ""; // если в ячейке объектов больше одного, то будет забирать инфу 
            // у подчинённого предмета и переведёт количество в текст
            // это позволит сплюсовать те элементы, которые у нас есть
        }
    }
        
    /// <summary>
    /// наш ItemInventory list
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

        Text[] texts = items[_id]._itemGameObject.GetComponentsInChildren<Text>();
        foreach (Text text in texts)
        {
            text.text = (invItem._count > 0 && invItem._id != 0) ? invItem._count.ToString() : "";
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

            RectTransform[] childRTs = newItem.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform childRT in childRTs)
            {
                childRT.localScale = Vector3.one;
            }
            // чтоб когда мы добавляем элементы и берём 
            // этот элемент, чтобы scale был такой же


            Button tempButton = newItem.GetComponent<Button>(); // каждый пункт инвенторя - кнопка
            // но можем нажимать только когда там что-то есть

            items.Add(ii); // добавляем элемент в ItemInventory
        }
    } 
}

[System.Serializable]
public class ItemInventory
{
    public int _id;
    public GameObject _itemGameObject; // тут мы можем поместить GameObj, который будем юзать
    public int _count;  // показывать, сколько в стаке элементов
}