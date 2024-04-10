using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBase : MonoBehaviour
{
    public List<Item> items = new List<Item>(); // здесь будет прописан каждый наш элемент
}

[System.Serializable] // поможет сохранять и загружать данные из объектов этого класса

public class Item // в классе 3 поля: id, name и img, которые мы можем сериализовать для сохранения и загрузки 
{
    public int _id; // уникальный первичный ключ
    public string _name; // так будет называться
    public Sprite _image; // иконка
}
