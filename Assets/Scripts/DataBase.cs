using System.Collections.Generic;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // each of our elements will be registered here
}

[System.Serializable] // will help you save and load data from objects of this class

public class Item // there are 3 fields in the class: id, name and image, which we can serialize for saving and loading 
{
    public int id; // unique primary key
    public string name; // name
    public Sprite image; // 
    public ItemType ItemType; 
}

public enum ItemType
{
    Empty, Patron, Money, Container
}
