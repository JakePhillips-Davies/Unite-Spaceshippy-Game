using System;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float sensitivity;

    private float yRot;
    private float xRot;

    [Header("")]
    [Header("Body reference")]
    [SerializeField] private GameObject body;

    [Header("")]
    [Header("Interaction")]
    [SerializeField] private int reach;        public int getReach() { return reach; }
    private RaycastHit hit;
    [SerializeField] private KeyCode interactkey;

    public Boolean ableToTurn { get; set; }


    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        yRot += Input.GetAxisRaw("Mouse X") * sensitivity;

        xRot -= Input.GetAxisRaw("Mouse Y") * sensitivity;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        body.transform.localRotation = Quaternion.Euler(0, yRot, 0);

        //activatableHandler();
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
