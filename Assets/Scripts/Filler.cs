using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class Filler
{
    public static void SearchForSameItem(List<ItemInventory> items, Item item, int count, int maxCount)
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (items[i].id == item.id && item.name != "Empty")
            {
                if (items[i].count < 128 && item.name != "Empty")
                {
                    items[i].count += count;

                    if (items[i].count > 128)
                    {
                        count = items[i].count - 128;
                        items[i].count = 64;
                    }
                    else
                    {
                        count = 0;
                        i = maxCount;
                    }
                }
            }
        }

        if (count > 0)
        {
            for (int i = 0; i < maxCount; i++)
            {
                if (items[i].id == 0)
                {
                    AddItem(items, i, item, count);
                    break;
                }
            }
        }
    }

    public static void AddItem(List<ItemInventory> items, int id, Item item, int count)
    {
        if (id >= 0 && id < items.Count)
        {
            items[id].id = item.id;
            items[id].count = count;
            items[id].itemGameObject.GetComponent<Image>().sprite = item.image;

            TextMeshProUGUI[] texts = items[id].itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                text.text = (count > 0 && item.id != 0) ? count.ToString() : "";
            }
        }
        else
        {
            Debug.LogError("Index out of bounds: " + id);
        }
    }
    
    public static void AddInventoryItem(List<ItemInventory> items, int id, ItemInventory invItem, DataBase data)
    {
        items[id].id = invItem.id;
        items[id].count = invItem.count;
        items[id].itemGameObject.GetComponent<Image>().sprite = data.items[invItem.id].image;

        TextMeshProUGUI[] texts = items[id].itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in texts)
        {
            text.text = (invItem.count > 0 && invItem.id != 0) ? invItem.count.ToString() : "";
        }
    }
}