using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingTool : MonoBehaviour
{
    [SerializeField] private PlayerCam player;
    [SerializeField] private PlaceableObjects placeableObjects;
    [SerializeField] private LayerMask mask;
    private RaycastHit hit;

    private void LateUpdate() {
        
        if(Physics.Raycast(transform.position, transform.forward, out hit, player.getReach(), mask) == false) return;

        if(Input.GetKeyDown(KeyCode.Mouse0)) Instantiate(placeableObjects.Current(), hit.point, new Quaternion(0, 0, 0, 0), hit.transform.parent);

    }

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * player.getReach());
        Gizmos.DrawSphere(hit.point, 0.05f);

    }
}
