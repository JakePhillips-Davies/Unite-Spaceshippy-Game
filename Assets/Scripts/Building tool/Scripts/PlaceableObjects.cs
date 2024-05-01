using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableObjects : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private List<GameObject> placeableObjects;
    private int selection;
    public bool hasChanged { get; set; }

    [Header("")]
    [Header("Keycodes")]
    [SerializeField] private KeyCode next;
    [SerializeField] private KeyCode previous;
    
    [Header("")]
    [Header("UI")]
    [SerializeField] private GameObject listUI;
    [SerializeField] private GameObject ItemUI;


    private void Start() {
        hasChanged = false;
        ListItems();
    }

    public void SetActive(int ID){
        selection = ID;
        hasChanged = true;
    }

    public GameObject Current() {

        return placeableObjects[selection];
    }

    private void ListItems(){

        for( int i = 0; i < placeableObjects.Count; i++)
        {
            Texture2D tex = placeableObjects[i].GetComponent<PlaceableObject>().tex;
        
            GameObject obj = Instantiate(ItemUI, listUI.transform);
            Text itemName = obj.transform.Find("ItemName").GetComponentInChildren<Text>();
            Image itemIcon = obj.transform.Find("ItemIcon").GetComponentInChildren<Image>();
            ItemID itemID = obj.GetComponent<ItemID>();

            itemID.ID = i;

            itemName.text = placeableObjects[i].name;

            itemIcon.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
        }
    }

}
