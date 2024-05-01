using UnityEngine;

public class Mover : MonoBehaviour
{
    public float force;
    Rigidbody rb;
    void OnTriggerStay(Collider other)
    {
        rb = other.GetComponent<Rigidbody>();
        rb.AddForce(transform.up * force, ForceMode.Force);
    }
}
