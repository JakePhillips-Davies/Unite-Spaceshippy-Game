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

    private Vector3 freeRotation;
    private quaternion finalRotation;

    private string snapTag;
    private bool snapped;

    private GameObject placing;
    private RaycastHit hit;
    private RaycastHit camHit;


    private void Start() {
        
        freeRotation = Vector3.zero;

    }
    private void Update() {
        
        if(placing != null) Destroy(placing);
        placing = null;

    }
    private void LateUpdate() {

        if(Physics.Raycast(transform.position, transform.forward, out hit, player.getReach(), mask) == false) return; // raycast forwards, return if it hits nothing

        CheckForSnapping();

        if( CheckPlaceability() == false ) return; // Return if not placeable anywhere and not snapped

        placing = Instantiate(placeableObjects.Current(), hit.point, finalRotation, hit.transform.parent);
        placing.GetComponentInChildren<Collider>().enabled = false;
        placing.transform.GetChild(0).GetComponent<Renderer>().material = tempPlacingMaterial;
        
        HandleRotation();
        
        Place();

    }

    private void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * player.getReach());
        Gizmos.DrawSphere(hit.point, 0.05f);

    }




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
        if(player.IsFreeLooking() && (snapped == false)){
            Instantiate(rotator, hit.point, Quaternion.Euler(freeRotation), placing.transform);
        }

        if(snapped){
            finalRotation = hit.collider.transform.rotation;
        }
        else{
            finalRotation = Quaternion.Euler(freeRotation);
        }

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
        if(Input.GetKeyDown(KeyCode.Mouse0) && (player.IsFreeLooking() == false)){
            if(snapped)
                Instantiate(placeableObjects.Current(), hit.point, finalRotation, hit.collider.transform).tag = "Placed";

            else
                Instantiate(placeableObjects.Current(), hit.point, finalRotation, hit.transform.parent).tag = "Placed";

        }
    }
    ////////////////////////////////////////////////////////////////////
}
