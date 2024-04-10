using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public List<ItemInventory> items = new List<ItemInventory>();

    public GameObject _gameObjShow; // какие-то геймОбжекты должны быть отражены
    // а некоторые просто стакаться и отправляться как доп подпункты и исчезать

    public GameObject InventoryMainObject; // надо подключить то, за что этот блок будет отвечать

    public int _maxCount;

    public void AddGraphics() // будет обновляться и считывать, какая картинка находится в ячейке и какие меняющиеся числа 
    {
        for (int i = 0; i < _maxCount; i++)
        {
            GameObject newItem = Instantiate(_gameObjShow, InventoryMainObject.transform) as GameObject;
            // новый экземпляр GameObj внутри InventoryMainObject как дочерний

            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory(); 
            ii._itemGameObj = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(0, 0, 0);    
            rt.localScale = new Vector3(1, 1, 1); // чтобы ячейки не скакали и были одинаковые

            RectTransform[] childTransforms = newItem.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform childTransform in childTransforms)
            {
                childTransform.localScale = new Vector3(1, 1, 1);
            }
            // чтоб когда мы добавляем элементы и берём 
            // этот элемент, чтобы scale был такой же

            Button tempButton = newItem.GetComponent<Button>(); // каждый пункт инвенторя - кнопка
            // но можем нажимать только когда там что-то есть

            items.Add(ii); // добавляем элемент в ItemInventory
        }
    }
}

[System.Serializable] // поможет сохранять и загружать данные из объектов этого класса

public class ItemInventory 
{
    public int _id; // для номера 
    public GameObject _itemGameObj; // тут мы можем поместить GameObj, который будем юзать
    public int _count; // показывать, сколько в стаке элементов
}
