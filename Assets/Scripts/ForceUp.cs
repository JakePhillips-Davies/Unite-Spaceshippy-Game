using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceUp : MonoBehaviour
{
    [SerializeField] int force;
    Rigidbody rb;
    private void FixedUpdate()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForceAtPosition(transform.forward * force, transform.right * 10);
    }
}
