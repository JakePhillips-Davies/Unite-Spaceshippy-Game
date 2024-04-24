using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DragBox : MonoBehaviour
{
    [SerializeField] Transform ship;

    public Rigidbody GetShipRB() {
        return ship.gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        transform.SetPositionAndRotation(ship.transform.position, ship.transform.rotation);
    }
}
