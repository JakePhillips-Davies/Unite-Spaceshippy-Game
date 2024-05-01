using System;
using UnityEngine;
using UnityEngine.UI;

public class VelocityReadoutPanel : MonoBehaviour
{
    [SerializeField] private Rigidbody ship;
    [SerializeField] private Text text;

    private void FixedUpdate() {
        text.text = String.Format("{0:0.00}" + " M/S", ship.velocity.magnitude);
    }
}
