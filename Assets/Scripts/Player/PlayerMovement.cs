using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private int walkSpeed;
    [SerializeField] private int sprintSpeed;
    [SerializeField] private int crouchSpeed;
    [SerializeField] private int airSpeed;

    [Space(7)]
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;


    [Space(7)]
    [SerializeField] private float jumpForce;

    [Space(7)]
    [Header("Key Binds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprint = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouch = KeyCode.LeftControl;


    private float forwardInput;
    private float rightInput;
    private Vector3 moveDirection;
    private float moveSpeed;

    private bool readyToJump;
    private bool safeToUncrouch;
    private bool crouching;

    public bool grounded;
    
    private RaycastHit hitInfo;
    private RaycastHit hitInfoDud;
    private Rigidbody rb;

    //---                                ---//
    private void OnTriggerEnter(Collider other) {
        transform.SetParent(other.transform.parent);
    }
    private void OnTriggerExit(Collider other) {
        transform.SetParent(null);
    }

    void Start() 
    {
        rb = GetComponent<Rigidbody>();

        readyToJump = true;
    }

    void FixedUpdate()
    {

        GroundCheck();

        if(Input.GetKey(crouch)){
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            crouching = true;
        }
        if(crouching){
            safeToUncrouch = !(Physics.SphereCast(transform.position, 0.495f, transform.up, out hitInfoDud, 1));
        }
        if(safeToUncrouch && !Input.GetKey(crouch)){
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            crouching = false;
        }

        GetMovement();

        if(Input.GetKey(jumpKey))
            Jump();

        MovePlayer();

        Drag();
    }

    void GetMovement()
    {
        if(crouching){
            moveSpeed = crouchSpeed;
        }
        else if(Input.GetKey(sprint)){
            moveSpeed = sprintSpeed;
        }
        else{
            moveSpeed = walkSpeed;
        }

        forwardInput = Input.GetAxisRaw("Vertical");
        rightInput = Input.GetAxisRaw("Horizontal");

        moveDirection = transform.forward * forwardInput + transform.right * rightInput;
        if(grounded){
            moveSpeed *= Mathf.Pow(1 + Vector3.Dot(moveDirection, hitInfo.normal), 1.3f); // make the player slower on steeper terrain by multiplying movespeed by the dot product + 1
            moveDirection = Vector3.ProjectOnPlane(moveDirection, hitInfo.normal);
        }
    }

    void MovePlayer()
    {
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 62, ForceMode.Force);
        else {
            rb.AddForce(moveDirection.normalized * airSpeed * 62, ForceMode.Force);
            rb.AddForce(-transform.up * 9.81f * 62, ForceMode.Force);
        }
    }

    void GroundCheck()
    {
        if(crouching){
            grounded = Physics.SphereCast(transform.position, 0.495f, -transform.up, out hitInfo, 0.02f);
        }
        else
            grounded = Physics.SphereCast(transform.position, 0.495f, -transform.up, out hitInfo, 0.51f);
    }

    void Drag()
    {
        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
        
    }

    void Jump()
    {
        if(grounded && readyToJump){
            readyToJump = false;
            grounded = false;

            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), 0.2f);
        }
    }
    void ResetJump()
    {
        readyToJump = true;
    }
}
