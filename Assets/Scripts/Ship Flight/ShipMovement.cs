using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    //---                      ---//
    public new GameObject camera;
    public GameObject ship;
    public GameObject shipInterior;
    private List<FuelInjector> engines; 

    //||-- movement --||//

    float forwardInput;
    float rightInput;
    float upInput;
    Vector3 shipForce;
    public int shipBaseSpeed;
    Rigidbody rb;

    //||-- movement --||//

    public KeyCode exit;

    [SerializeField] private KeyCode boost;
    
    public KeyCode freeCam;

    //---                      ---//

    private void Start() {
        engines = new List<FuelInjector>();
    }

    private void Awake() {
        shipInterior.GetComponentsInChildren<FuelInjector>(engines);
    }
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Input.mousePosition.Set(Screen.width * 0.5f, Screen.height * 0.5f, 0f);

        rb = ship.GetComponent<Rigidbody>();

        rb.drag = 0.5f;

        shipInterior.GetComponentsInChildren<FuelInjector>(engines);
    }

    void Update()
    {
        if(Input.GetKeyDown(exit)){
            ControlSwapper cs = GetComponent<ControlSwapper>();
            cs.Activate();
        }

        if(Input.GetKeyDown(boost)) {
            shipInterior.GetComponentsInChildren<FuelInjector>(engines);
            foreach (FuelInjector engine in engines)
            {
                engine.enabled = true;
                Debug.Log(engine.name);
            }
        }
        if(Input.GetKeyUp(boost)) {
            foreach (FuelInjector engine in engines)
            {
                engine.enabled = false;
            }
        }


        if(Input.GetKeyDown(freeCam))
        {
            ShipFreeCam cam = camera.GetComponent<ShipFreeCam>();
            cam.enabled = !cam.enabled;
        }
    }

    void FixedUpdate()
    {
        forwardInput = -Input.GetAxisRaw("Vertical");
        rightInput = -Input.GetAxisRaw("Horizontal");
        upInput = Input.GetAxisRaw("UppyDowny");

        shipForce = (transform.forward * forwardInput + transform.right * rightInput + transform.up * upInput) * shipBaseSpeed * rb.mass;

        rb.AddForce(shipForce);
    }
}
