using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingPoint : MonoBehaviour
{
    private void OnDrawGizmos() {
        
        Gizmos.DrawRay(transform.position, transform.forward * 5);

        Gizmos.DrawWireSphere(transform.position, 0.1f);

    }
}
