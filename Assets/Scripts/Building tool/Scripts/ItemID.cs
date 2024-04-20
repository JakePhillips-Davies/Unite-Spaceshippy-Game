using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemID : MonoBehaviour
{
    public int ID;

    public void SetActive(){

        FindFirstObjectByType<PlaceableObjects>().SetActive(ID);
        
    }
}
