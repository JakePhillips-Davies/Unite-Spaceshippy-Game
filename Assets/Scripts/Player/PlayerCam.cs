using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sensitivity;
    private float yRot;
    private float xRot;
    [SerializeField] private KeyCode freeMouseKey;      public bool IsFreeLooking() { return Input.GetKey(freeMouseKey); }

    [Header("")]
    [Header("Body reference")]
    [SerializeField] private GameObject body;

    [Header("")]
    [Header("Interaction")]
    [SerializeField] private int reach;        public int getReach() { return reach; }
    [SerializeField] private KeyCode interactkey;
    private RaycastHit hit;
    private Ray interactRay;        public Ray GetInteractRay() { return interactRay; }

    public Boolean ableToTurn { get; set; }

    private void Start() {
        ableToTurn = true;
    }

    void Update()
    {
        if(Input.GetKey(freeMouseKey)){
            Cursor.lockState = CursorLockMode.Confined;
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            body.transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }
        else if(ableToTurn){
            Cursor.lockState = CursorLockMode.Locked;

            yRot += Input.GetAxisRaw("Mouse X") * sensitivity;

            xRot -= Input.GetAxisRaw("Mouse Y") * sensitivity;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            body.transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            body.transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }

        interactRay = Camera.main.ScreenPointToRay (Input.mousePosition);

        //activatableHandler();
    }

    private void OnDrawGizmos() {
        
        Gizmos.DrawRay(transform.position, interactRay.direction * reach);

    }

    // void activatableHandler()
    // {
    //     text.text = "";
    //     panel.SetActive(false);

    //     if(Physics.Raycast(transform.position, transform.forward, out hit, reach)){
    //         if(Input.GetKeyDown(activateKey) && (hit.transform != null) && (hit.collider.GetComponent<Activator>() != null))
    //             hit.collider.GetComponent<Activator>().Activate();

    //         if((hit.transform != null) && (hit.collider.GetComponent<Activator>() != null)){
    //             panel.SetActive(true);
    //             text.text = String.Format("{0}", hit.collider.name);
    //         }
    //     }
    // }
}
