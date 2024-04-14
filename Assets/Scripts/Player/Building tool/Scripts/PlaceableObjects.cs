using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlaceableObjects : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private List<GameObject> placeableObjects;
    private int selection;
    private bool hasChanged = false; public bool GetHasChanged() { return hasChanged; } 

    [Header("")]
    [Header("Keycodes")]
    [SerializeField] private KeyCode next;
    [SerializeField] private KeyCode previous;

    private void Update() {

        hasChanged = false;
        
        if(Input.GetKeyDown(next)) {selection++; hasChanged = true;}

        if(Input.GetKeyDown(previous)) {selection--; hasChanged = true;}

        if(selection < 0) selection = placeableObjects.Count - 1;

        if(selection > placeableObjects.Count - 1) selection = 0;

    }

    public GameObject Current() {

        return placeableObjects[selection];

    }

}
