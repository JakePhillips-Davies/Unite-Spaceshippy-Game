using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{

    [Header("Atributes")]
    [SerializeField] private bool placeableAnywhere;
    public bool GetPlaceableAnywhere() { return placeableAnywhere; }
    [SerializeField] private List<String> tags;
    public List<String> GetTagList() { return tags; }

}
