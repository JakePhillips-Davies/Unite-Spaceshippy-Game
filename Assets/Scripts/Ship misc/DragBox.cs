using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DragBox : MonoBehaviour
{
    [SerializeField] Transform ship;

    public Boolean tmp;

    private void FixedUpdate() {
        transform.SetPositionAndRotation(ship.transform.position, ship.transform.rotation);
    }
}
