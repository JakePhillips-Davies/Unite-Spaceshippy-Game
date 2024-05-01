using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{

    [Header("Atributes")]
    [SerializeField] private bool placeableAnywhere;
    public Texture2D tex;
    public bool GetPlaceableAnywhere() { return placeableAnywhere; }
    [SerializeField] private List<String> tags;
    public List<String> GetTagList() { return tags; }

}
