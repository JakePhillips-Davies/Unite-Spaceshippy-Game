using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingTool : MonoBehaviour
{
    [Header("------------")]
    [SerializeField] private PlayerCam player;
    [SerializeField] private GameObject rotator;
    [SerializeField] private PlaceableObjects placeableObjects;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Material tempPlacingMaterial;
    [SerializeField] private Material tempPlacingMaterialRed;

    private GameObject rotationAxis;
    private bool isGrabbing;
    float lastAngle;
    Vector3 intesectionPoint;
    Vector3 firstVectorToIntersectionPoint;

    private string snapTag;
    private bool snapped;

    private GameObject placing;
    private bool isPlaceable;

    private RaycastHit hit;
    private RaycastHit camHit;


    private void LateUpdate() {

        if(Physics.Raycast(transform.position, transform.forward, out hit, player.getReach(), mask) == false) {
            DestroyPlacing();
            return; // raycast forwards, return if it hits nothing
        }

        CheckForSnapping();

        isPlaceable = CheckPlaceability();

        if(placeableObjects.GetHasChanged()) DestroyPlacing();

        if(placing == null){
            placing = Instantiate(placeableObjects.Current(), hit.point, new quaternion(0, 0, 0, 0), hit.transform.parent);
            foreach (var collider in placing.GetComponentsInChildren<Collider>())
                collider.enabled = false;
        }
        else{
            placing.transform.position = hit.point;
        }

        foreach (Renderer renderer in placing.GetComponentsInChildren<Renderer>()){
            if(renderer.gameObject.layer == 25) ;
            else if(isPlaceable) renderer.material = tempPlacingMaterial;
            else renderer.material = tempPlacingMaterialRed;
        }
        
        HandleRotation();
        
        Place();

    }

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * player.getReach());
        Gizmos.DrawSphere(hit.point, 0.05f);

        if( rotationAxis != null ){
            Gizmos.DrawRay(rotationAxis.transform.position, rotationAxis.transform.up * 10);
            Gizmos.DrawSphere(intesectionPoint, 0.05f);
            Gizmos.DrawLine(rotationAxis.transform.position, intesectionPoint);
            Gizmos.DrawLine(rotationAxis.transform.position, rotationAxis.transform.position + firstVectorToIntersectionPoint);
        }

    }


    //------------------------- Destroy placing -------------------------//
    private void DestroyPlacing() {
        
        if(placing != null) Destroy(placing);
        placing = null;

    }
    ///////////////////////////////////////////////////////////////////////
    

    //------------------------- Check For Snapping -------------------------//
    private void CheckForSnapping()
    {
        if(hit.collider.transform.gameObject.layer == LayerMask.NameToLayer("Snapping Point")) { // if hit's layer is "Snapping Point"

            hit.point = hit.collider.transform.position; // snap to a snapping point if it is hit
            snapTag = hit.collider.gameObject.tag;
            snapped = true;

        }
        else snapped = false;
    }
    //////////////////////////////////////////////////////////////////////////
    

    //------------------------- Handle Rotation -------------------------//
    private void HandleRotation()
    {
        if(snapped){
            placing.transform.rotation = new quaternion(0, 0, 0, 0);
        }

        else if(player.IsFreeLooking() && (snapped == false)){

            if( placing.GetComponentInChildren<RotationHandler>() == null ) Instantiate(rotator, hit.point, new quaternion(0, 0, 0, 0), placing.transform);

            if(Input.GetKeyDown(KeyCode.Mouse0)){

                if( Physics.Raycast(player.GetInteractRay(), out camHit, player.getReach(), LayerMask.GetMask("Rotate Handle")) == false ) return;

                rotationAxis = camHit.collider.gameObject;

                isGrabbing = true;

            }
            if(Input.GetKeyUp(KeyCode.Mouse0)){

                isGrabbing = false;

            }
            if(Input.GetKey(KeyCode.Mouse0) && isGrabbing){

                Plane plane = new Plane(rotationAxis.transform.up, rotationAxis.transform.position);

                float distance;

                if( plane.Raycast(player.GetInteractRay(), out distance) == false ) return;

                intesectionPoint = player.GetInteractRay().origin + player.GetInteractRay().direction * distance;

                
                if(Input.GetKeyDown(KeyCode.Mouse0)) {
                    lastAngle = 0;
                    firstVectorToIntersectionPoint = intesectionPoint - rotationAxis.transform.position;
                }

                float currentAngle = Vector3.SignedAngle(firstVectorToIntersectionPoint, intesectionPoint - rotationAxis.transform.position, rotationAxis.transform.up);
                float deltaAngle = currentAngle - lastAngle;
                lastAngle = currentAngle;
                
                // Calculate the rotation dependant on what axis you selected
                placing.transform.Rotate(Vector3.Dot(placing.transform.right, rotationAxis.transform.up)*deltaAngle, Vector3.Dot(placing.transform.up, rotationAxis.transform.up)*deltaAngle, Vector3.Dot(placing.transform.forward, rotationAxis.transform.up)*deltaAngle);
                
            }
            
        }
        
        else if( placing.GetComponentInChildren<RotationHandler>() != null ) Destroy(placing.GetComponentInChildren<RotationHandler>().gameObject);
    }
    ///////////////////////////////////////////////////////////////////////


    //------------------------- Check Placeability -------------------------//
    private bool CheckPlaceability()
    {
        PlaceableObject placeableObject = placeableObjects.Current().GetComponent<PlaceableObject>();

        if( snapped ){

            if( hit.collider.transform.childCount > 0 ) return false;

            foreach (var tag in placeableObject.GetTagList())
            {
                if(tag.Equals(snapTag)) return true;
            }
            
            return false;

        }
        else{ // if not snapped

            if( placeableObject.GetPlaceableAnywhere() == false ) return false; // if not placeable anywhere return false

            return true;

        }
    }
    //////////////////////////////////////////////////////////////////////////
    

    //------------------------- Place object -------------------------//
    private void Place()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && isPlaceable && (player.IsFreeLooking() == false)){
            if(snapped){
                Instantiate(placeableObjects.Current(), hit.point, placing.transform.rotation, hit.collider.transform).tag = "Placed";
                hit.collider.enabled = false;
            }

            else
                Instantiate(placeableObjects.Current(), hit.point, placing.transform.rotation, hit.transform.parent).tag = "Placed";

        }
    }
    ////////////////////////////////////////////////////////////////////
}
