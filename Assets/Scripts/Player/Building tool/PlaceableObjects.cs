using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlaceableObjects : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private List<GameObject> placeableObjects;
    private int selection;

    [Header("")]
    [Header("Keycodes")]
    [SerializeField] private KeyCode next;
    [SerializeField] private KeyCode previous;

    private void Update() {
        
        if(Input.GetKeyDown(next)) selection++;

        if(Input.GetKeyDown(previous)) selection--;

        if(selection < 0) selection = placeableObjects.Count - 1;

        if(selection > placeableObjects.Count - 1) selection = 0;

    }

    public GameObject Current() {

        return placeableObjects[selection];

    }

}
