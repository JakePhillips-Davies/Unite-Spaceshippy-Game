using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

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
    Quaternion initialRotation;
    Vector3 intesectionPoint;
    Vector3 firstVectorToIntersectionPoint;

    private string extensionState;
    private Vector3 extensionFromPosition;
    private Vector3 extensionDirection;
    private float extensionDistance;
    private Collider pipeStartSnappingPoint;
    private Quaternion startingRotation;
    private GameObject alignmentObj;
    [SerializeField] private GameObject alignModePrompt; 
    public bool alignMode { get; set; } = false;

    public bool roundingPosition { get; set; } = true;
    private string snapTag;
    private bool snapped;

    private GameObject objectToBePlaced;
    private bool isPlaceable;

    private RaycastHit hit;
    private RaycastHit camHit;

    private void Start() {
        extensionState = "plopping";
        alignModePrompt.SetActive(false);
        alignMode = false;
    }

    private void LateUpdate() {

        switch (placeableObjects.Current().name)
        {
            case "Pipe Fuel":
                PlacePipe();
                break;
            case "Pipe Coolant":
                PlacePipe();
                break;

            default:
                PlaceDefault();
                break;
        }

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
    
    private void OnDisable() {

        DestroyPlacing();

        extensionState = "plopping";

        alignModePrompt.SetActive(false);
        alignModePrompt.GetComponentInChildren<Toggle>().isOn = false;
        alignMode = false;

        PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();
        
    }

    private void PlacePipe(){

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        if( extensionState == "plopping" ){

            if(Physics.Raycast(transform.position, transform.forward, out hit, player.getReach(), mask) == false) {
                DestroyPlacing();
                return; // raycast forwards, return if it hits nothing
            }

            CheckForSnapping();

            isPlaceable = CheckPlaceability();

            if(placeableObjects.hasChanged) {
                DestroyPlacing();
                placeableObjects.hasChanged = false;
            }

            if(objectToBePlaced == null){
                objectToBePlaced = Instantiate(placeableObjects.Current(), hit.point, new quaternion(0, 0, 0, 0), hit.transform.parent);
                foreach (var collider in objectToBePlaced.GetComponentsInChildren<Collider>())
                    collider.enabled = false;
            }
            else{
                objectToBePlaced.transform.position = hit.point;
            }

            objectToBePlaced.transform.position += objectToBePlaced.transform.forward * 0.25f;

            if( !snapped && roundingPosition ) {
                RoundPosition();
            }

            if( snapped ) {
                objectToBePlaced.transform.rotation = hit.collider.transform.rotation;
                isPlaceable = ( objectToBePlaced.GetComponentInChildren<FluidContainer>().GetFluidType() == hit.collider.gameObject.GetComponentInChildren<FluidOutput>().GetOutputsFrom().GetFluidType() );
            }

            foreach (Renderer renderer in objectToBePlaced.GetComponentsInChildren<Renderer>()){
                if(renderer.gameObject.layer == 25) {}
                else if(isPlaceable) renderer.material = tempPlacingMaterial;
                else renderer.material = tempPlacingMaterialRed;
            }

            if( isPlaceable && Input.GetKeyDown(KeyCode.Mouse0) ) {
                alignModePrompt.SetActive(true);
                alignModePrompt.GetComponentInChildren<Toggle>().isOn = false;
                alignMode = false;

                extensionState = "extending";

                pipeStartSnappingPoint = hit.collider;
                startingRotation = objectToBePlaced.transform.rotation;
                
                snapped = false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        else if( extensionState == "extending" ){

            if(alignMode) PipeInputsSingleton.pipeInputs.DisplayInputSnaps();
            else  PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();

            Transform pipeConnector = objectToBePlaced.transform.Find("Point towards").Find("Pipe Extender");

            if(Physics.Raycast(transform.position, transform.forward, 100, LayerMask.GetMask("Snapping Point Pipe Input")) && Physics.Raycast(pipeStartSnappingPoint.transform.position, pipeStartSnappingPoint.transform.forward, out hit, 20, LayerMask.GetMask("Snapping Point Pipe Input"))){

                objectToBePlaced.transform.position = hit.collider.transform.position;

                objectToBePlaced.transform.Find("Pipe connection to").gameObject.SetActive(false);

                pipeConnector.localScale = new Vector3(pipeConnector.localScale.x, (pipeStartSnappingPoint.transform.position - objectToBePlaced.transform.position).magnitude - 0.25f, pipeConnector.localScale.z);

                isPlaceable = ( objectToBePlaced.GetComponentInChildren<FluidContainer>().GetFluidType() == hit.collider.gameObject.GetComponent<FluidInput>().GetInputsTo().GetFluidType() );
                foreach (Renderer renderer in objectToBePlaced.GetComponentsInChildren<Renderer>()){
                    if(renderer.gameObject.layer == 25) {}
                    else if(isPlaceable) renderer.material = tempPlacingMaterial;
                    else renderer.material = tempPlacingMaterialRed;
                }
                
                if( Input.GetKeyDown(KeyCode.Mouse0) && !player.isFreeLooking && isPlaceable) {

                    GameObject placing = Instantiate(placeableObjects.Current(), objectToBePlaced.transform.position, objectToBePlaced.transform.rotation, pipeStartSnappingPoint.transform.parent.parent);
                    placing.tag = "Placed";
                    placing.transform.Find("Point towards").Find("Pipe Extender").localScale = objectToBePlaced.transform.Find("Point towards").Find("Pipe Extender").localScale;
                    placing.transform.Find("Point towards").rotation = objectToBePlaced.transform.Find("Point towards").rotation;
                    placing.transform.Find("Pipe connection to").Find("SnappingPoint").Find("Output").parent = placing.transform;
                    Destroy(placing.transform.Find("Pipe connection to").gameObject);

                    pipeStartSnappingPoint.gameObject.GetComponentInChildren<FluidOutput>().SetOutputsTo(placing.GetComponentInChildren<FluidContainer>());
                    placing.GetComponentInChildren<FluidOutput>().SetOutputsTo(hit.collider.gameObject.GetComponent<FluidInput>().GetInputsTo());

                    extensionState = "plopping";

                    alignModePrompt.SetActive(false);
                    alignModePrompt.GetComponentInChildren<Toggle>().isOn = false;
                    alignMode = false;

                    pipeStartSnappingPoint.enabled = false;

                    hit.collider.enabled = false;

                    PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();

                    DestroyPlacing();

                }

            }
            else {
                objectToBePlaced.transform.Find("Pipe connection to").gameObject.SetActive(true);

                isPlaceable = true;

                foreach (Renderer renderer in objectToBePlaced.GetComponentsInChildren<Renderer>()){
                    if(renderer.gameObject.layer == 25) {}
                    else if(isPlaceable) renderer.material = tempPlacingMaterial;
                    else renderer.material = tempPlacingMaterialRed;
                }

                extensionDirection = pipeStartSnappingPoint.transform.forward;
                extensionFromPosition = pipeStartSnappingPoint.transform.position;

                Vector3 extentionToPlayer = player.transform.position - extensionFromPosition;
                float extentionToPlayerDistance = extentionToPlayer.magnitude;
                float angleFromExtensionToPlayer = Vector3.Angle(extensionDirection, extentionToPlayer);
                float angleFromPlayerToExtension = Vector3.Angle(-extentionToPlayer, player.transform.forward);

                extensionDistance = extentionToPlayerDistance * Mathf.Sin(angleFromPlayerToExtension * Mathf.PI / 180) / Mathf.Sin((180 - angleFromPlayerToExtension - angleFromExtensionToPlayer) * Mathf.PI / 180);
                if(extensionDistance >= 20) extensionDistance = 20;
                else if(extensionDistance <= 0) extensionDistance = 0;

                objectToBePlaced.transform.position = extensionFromPosition + extensionDirection * ( extensionDistance + 0.25f );
                pipeConnector.localScale = new Vector3(pipeConnector.localScale.x, extensionDistance, pipeConnector.localScale.z);

                HandleRotation(false, false, true);

                if( Input.GetKeyDown(KeyCode.Mouse0) && !player.isFreeLooking ) extensionState = "rotating";
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        else if( extensionState == "rotating" ){

            if(alignMode) PipeInputsSingleton.pipeInputs.DisplayInputSnaps();
            else  PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();

            Transform pipeToStart = objectToBePlaced.transform.Find("Point towards");

            if( alignMode ){

                if( objectToBePlaced.GetComponentInChildren<RotationHandler>() != null ) Destroy(objectToBePlaced.GetComponentInChildren<RotationHandler>().gameObject);

                Physics.Raycast(transform.position, transform.forward, out hit, 100, mask);

                if(hit.collider.tag == "Pipe Align Snap"){
                    Vector3 extentionToPlayer = player.transform.position - hit.collider.transform.position;
                    float extentionToPlayerDistance = extentionToPlayer.magnitude;
                    float angleFromExtensionToPlayer = Vector3.Angle(-hit.collider.transform.up, extentionToPlayer);
                    float angleFromPlayerToExtension = Vector3.Angle(-extentionToPlayer, player.transform.forward);

                    float extensionDistanceT = extentionToPlayerDistance * Mathf.Sin(angleFromPlayerToExtension * Mathf.PI / 180) / Mathf.Sin((180 - angleFromPlayerToExtension - angleFromExtensionToPlayer) * Mathf.PI / 180);
                    hit.point = hit.collider.transform.position - hit.collider.transform.up*extensionDistanceT;
                
                    objectToBePlaced.transform.LookAt(hit.point);
                    objectToBePlaced.transform.Rotate(new Vector3(-90, 0, 0));


                    isPlaceable = ( objectToBePlaced.GetComponentInChildren<FluidContainer>().GetFluidType() == hit.collider.transform.parent.gameObject.GetComponent<FluidInput>().GetInputsTo().GetFluidType() );


                    if(alignmentObj == null){
                        alignmentObj = Instantiate(placeableObjects.Current(), hit.point, new quaternion(0, 0, 0, 0), objectToBePlaced.transform);
                        foreach (var collider in alignmentObj.GetComponentsInChildren<Collider>())
                            collider.enabled = false;
                        alignmentObj.transform.LookAt(hit.collider.transform.position);
                        alignmentObj.transform.Rotate(new Vector3(-90, 0, 0));
                    }
                    else{
                        alignmentObj.transform.position = hit.point;
                        alignmentObj.transform.LookAt(hit.collider.transform.position);
                        alignmentObj.transform.Rotate(new Vector3(-90, 0, 0));
                    }
                    foreach (Renderer renderer in alignmentObj.GetComponentsInChildren<Renderer>()){
                        if(renderer.gameObject.layer == 25) {}
                        else if(isPlaceable) renderer.material = tempPlacingMaterial;
                        else renderer.material = tempPlacingMaterialRed;
                    }
                    alignmentObj.transform.Find("Point towards").LookAt(objectToBePlaced.transform.position);
                    alignmentObj.transform.Find("Point towards").rotation *= Quaternion.Euler(0, 180, 0);
                    alignmentObj.transform.Find("Point towards").Find("Pipe Extender").localScale = new Vector3(alignmentObj.transform.Find("Point towards").Find("Pipe Extender").localScale.x, (objectToBePlaced.transform.position - hit.point).magnitude - 0.5f, alignmentObj.transform.Find("Point towards").Find("Pipe Extender").localScale.z);

                }

            }
            else {
                if(alignmentObj != null) Destroy(alignmentObj);
                isPlaceable = true;
                alignmentObj = null;
                HandleRotation(true, false, false);
            }

            foreach (Renderer renderer in objectToBePlaced.GetComponentsInChildren<Renderer>()){
                if(renderer.gameObject.layer == 25) {}
                else if(isPlaceable) renderer.material = tempPlacingMaterial;
                else renderer.material = tempPlacingMaterialRed;
            }

            pipeToStart.LookAt(pipeStartSnappingPoint.transform.position);
            pipeToStart.rotation *= Quaternion.Euler(0, 180, 0);

            if( Input.GetKeyDown(KeyCode.Mouse0) && !player.isFreeLooking && isPlaceable) {
                GameObject placing = Instantiate(placeableObjects.Current(), objectToBePlaced.transform.position, objectToBePlaced.transform.rotation, pipeStartSnappingPoint.transform.parent.parent);
                placing.tag = "Placed";
                placing.transform.Find("Point towards").Find("Pipe Extender").localScale = objectToBePlaced.transform.Find("Point towards").Find("Pipe Extender").localScale;
                placing.transform.Find("Point towards").rotation = objectToBePlaced.transform.Find("Point towards").rotation;

                if(alignmentObj != null){

                    Destroy(placing.transform.Find("Pipe connection to").Find("SnappingPoint").gameObject);
                    Destroy(placing.transform.Find("FuelContainer").gameObject);

                    Transform parent = placing.transform.Find("Pipe connection to");

                    placing = Instantiate(placeableObjects.Current(), alignmentObj.transform.position, alignmentObj.transform.rotation, parent);
                    placing.tag = "Placed";
                    placing.transform.Find("Point towards").Find("Pipe Extender").localScale = alignmentObj.transform.Find("Point towards").Find("Pipe Extender").localScale;
                    placing.transform.Find("Point towards").rotation = alignmentObj.transform.Find("Point towards").rotation;

                }

                pipeStartSnappingPoint.gameObject.GetComponentInChildren<FluidOutput>().SetOutputsTo(placing.GetComponentInChildren<FluidContainer>());

                extensionState = "plopping";

                alignModePrompt.SetActive(false);
                alignModePrompt.GetComponentInChildren<Toggle>().isOn = false;
                alignMode = false;

                pipeStartSnappingPoint.enabled = false;

                PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();

                DestroyPlacing();
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
    }
    private void PlaceDefault(){

        if(Physics.Raycast(transform.position, transform.forward, out hit, player.getReach(), mask) == false) {
            DestroyPlacing();
            return; // raycast forwards, return if it hits nothing
        }

        CheckForSnapping();

        isPlaceable = CheckPlaceability();

        if(placeableObjects.hasChanged) {
            DestroyPlacing();

            extensionState = "plopping";
            alignModePrompt.SetActive(false);
            alignModePrompt.GetComponentInChildren<Toggle>().isOn = false;
            alignMode = false;
            PipeInputsSingleton.pipeInputs.DestroyDisplayInputSnaps();

            placeableObjects.hasChanged = false;
        }

        if(objectToBePlaced == null){
            objectToBePlaced = Instantiate(placeableObjects.Current(), hit.point, new quaternion(0, 0, 0, 0), hit.transform.parent);
            foreach (var collider in objectToBePlaced.GetComponentsInChildren<Collider>())
                collider.enabled = false;
        }
        else{
            objectToBePlaced.transform.parent = hit.transform.parent;
            objectToBePlaced.transform.position = hit.point;
        }

        if( !snapped && roundingPosition ) {
            RoundPosition();
        }

        foreach (Renderer renderer in objectToBePlaced.GetComponentsInChildren<Renderer>()){
            if(renderer.gameObject.layer == 25) {}
            else if(isPlaceable) renderer.material = tempPlacingMaterial;
            else renderer.material = tempPlacingMaterialRed;
        }
        
        HandleRotation(true, true, true);
        
        Place();

    }


    //------------------------- Destroy placing -------------------------//
    private void DestroyPlacing() {
        
        if(objectToBePlaced != null) Destroy(objectToBePlaced);
        objectToBePlaced = null;

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
    private void HandleRotation(bool x, bool y, bool z)
    {
        if(snapped){
            objectToBePlaced.transform.rotation = hit.collider.transform.rotation;
        }

        else if(player.isFreeLooking && (snapped == false)){

            if( objectToBePlaced.GetComponentInChildren<RotationHandler>() == null ) {

                GameObject rotatorInst = Instantiate(rotator, objectToBePlaced.transform.position, new quaternion(0, 0, 0, 0), objectToBePlaced.transform);

                rotatorInst.transform.Find("RotateX").gameObject.SetActive(x);
                rotatorInst.transform.Find("RotateY").gameObject.SetActive(y);
                rotatorInst.transform.Find("RotateZ").gameObject.SetActive(z);

            }

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
                    firstVectorToIntersectionPoint = intesectionPoint - rotationAxis.transform.position;
                    initialRotation = objectToBePlaced.transform.rotation;
                }

                float currentAngle = Vector3.SignedAngle(firstVectorToIntersectionPoint, intesectionPoint - rotationAxis.transform.position, rotationAxis.transform.up);
                float snappedAngle = Mathf.Round(currentAngle / 15) * 15;
                
                objectToBePlaced.transform.rotation = initialRotation;
                // Calculate the rotation dependant on what axis you selected
                objectToBePlaced.transform.Rotate(Vector3.Dot(objectToBePlaced.transform.right, rotationAxis.transform.up)*snappedAngle, Vector3.Dot(objectToBePlaced.transform.up, rotationAxis.transform.up)*snappedAngle, Vector3.Dot(objectToBePlaced.transform.forward, rotationAxis.transform.up)*snappedAngle);
                
            }
            
        }
        
        else if( objectToBePlaced.GetComponentInChildren<RotationHandler>() != null ) Destroy(objectToBePlaced.GetComponentInChildren<RotationHandler>().gameObject);
    }
    ///////////////////////////////////////////////////////////////////////
    
    //------------------------- Round Position -------------------------//
    private void RoundPosition()
    {
        float x = Mathf.Round(objectToBePlaced.transform.localPosition.x * 4) / 4;
        float y = Mathf.Round(objectToBePlaced.transform.localPosition.y * 4) / 4;
        float z = Mathf.Round(objectToBePlaced.transform.localPosition.z * 4) / 4;

        objectToBePlaced.transform.localPosition = new Vector3(x, y, z);
    }
    //////////////////////////////////////////////////////////////////////


    //------------------------- Check Placeability -------------------------//
    private bool CheckPlaceability()
    {
        PlaceableObject placeableObject = placeableObjects.Current().GetComponent<PlaceableObject>();

        if( snapped ){

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
        if(Input.GetKeyDown(KeyCode.Mouse0) && isPlaceable && (player.isFreeLooking == false)){
            if(snapped){
                Instantiate(placeableObjects.Current(), objectToBePlaced.transform.position, objectToBePlaced.transform.rotation, hit.transform.parent).tag = "Placed";
                hit.collider.enabled = false;
            }

            else
                Instantiate(placeableObjects.Current(), objectToBePlaced.transform.position, objectToBePlaced.transform.rotation, hit.transform.parent).tag = "Placed";

        }
    }
    ////////////////////////////////////////////////////////////////////
}
