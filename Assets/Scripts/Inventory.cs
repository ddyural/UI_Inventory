using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public DataBase data;
    public List<ItemInventory> items = new List<ItemInventory>();
    public GameObject inventorySlotPrefab;
    public GameObject inventoryMainObject;
    public int maxCount;
    public Camera cam;
    public EventSystem es;
    public int currentID;
    public ItemInventory currentItem;
    public RectTransform movingObject;
    public Vector3 offset;
    private RectTransform _rectTransform;
    private Dictionary<int, TextMeshProUGUI[]> _textComponents = new Dictionary<int, TextMeshProUGUI[]>();
    private Dictionary<int, Image[]> _imageComponents = new Dictionary<int, Image[]>();

    public void Start()
    {
        AddGraphics();
        _rectTransform = inventoryMainObject.GetComponent<RectTransform>();
        for (int i = 0; i < maxCount; i++)
        {
            Item randomItem = data.items[Random.Range(0, data.items.Count)];
            int randomCount = Random.Range(1, 99);
            Filler.AddItem(items, i, randomItem, randomCount);
        }
        
        //graphicsHandler.InitializeInventory(items, inventorySlotPrefab, inventoryMainObject);
    }

    public void Update()
    {
        if (currentID != -1)
        {
            MoveObject();
        }
        UpdateInventory();
    }
    
    public void AddGraphics()
    {
        if (inventorySlotPrefab == null || inventoryMainObject == null)
        {
            Debug.LogError("InventoryGraphicsHandler: inventorySlotPrefab or inventoryMainObject is not initialized!");
            return;
        }

        for (int i = 0; i < maxCount; i++)
        {
            GameObject newItem = Instantiate(inventorySlotPrefab, inventoryMainObject.transform);
            newItem.name = i.ToString();

            ItemInventory ii = new ItemInventory();
            ii.itemGameObject = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            RectTransform[] childRTs = newItem.GetComponentsInChildren<RectTransform>();
            foreach (RectTransform childRT in childRTs)
            {
                childRT.localScale = Vector3.one;
            }

            Button tempButton = newItem.GetComponent<Button>();
            tempButton.onClick.AddListener(delegate { SelectObject(); });

            items.Add(ii);
        }
    }
    
    public void MoveObject()
    {
        Vector3 pos = Input.mousePosition + offset;
        pos.z = _rectTransform.position.z;
        movingObject.position = cam.ScreenToWorldPoint(pos);
    }
    
    public void UpdateItemComponents(int itemId)
    {
        GameObject itemGameObject = items[itemId].itemGameObject;
        
        TextMeshProUGUI[] texts = itemGameObject.GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = itemGameObject.GetComponentsInChildren<Image>();
        
        _textComponents[itemId] = texts;
        _imageComponents[itemId] = images;
    }
    
    public void UpdateInventory()
    {
        for (int i = 0; i < maxCount; i++)
        {
            if (_textComponents.ContainsKey(i) && _imageComponents.ContainsKey(i))
            {
                if (items[i].id != 0 && items[i].count > 1)
                {
                    foreach (TextMeshProUGUI text in _textComponents[i])
                    {
                        text.text = items[i].count.ToString();
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
                    image.sprite = data.items[items[i].id].image;
                }
            }
        }
    }

    public void SelectObject()
    {
        int selectedId = int.Parse(es.currentSelectedGameObject.name);
        ItemInventory selectedInventoryItem = items[selectedId];
        
        if (currentID == -1) 
        {
            if (selectedInventoryItem.id == 0)
            {
                return;
            }
            
            currentID = int.Parse(es.currentSelectedGameObject.name); 
            currentItem = CopyInventoryItem(items[currentID]); 
            movingObject.gameObject.SetActive(true);
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].image; 

            Filler.AddItem(items, currentID, data.items[0], 0); 
        }
        else
        {
            ItemInventory II = items[int.Parse(es.currentSelectedGameObject.name)];

            if (currentItem.id != II.id)
            {
                Filler.AddInventoryItem(items, currentID, II, data);

                Filler.AddInventoryItem(items,int.Parse(es.currentSelectedGameObject.name), currentItem, data);
            }
            else
            {
                if (II.count + currentItem.count <= 128) 
                {
                    II.count += currentItem.count;
                }
                else
                {
                    Filler.AddItem(items, currentID, data.items[II.id], II.count + currentItem.count - 128); 

                    II.count = 128;
                }
                II.itemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = II.count.ToString();

            }

            currentID = -1;
            movingObject.gameObject.SetActive(false); 
        }
    }
    
    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory New = new ItemInventory();
        New.id = old.id;
        New.itemGameObject = old.itemGameObject;
        New.count = old.count;
        return New;
    }
}