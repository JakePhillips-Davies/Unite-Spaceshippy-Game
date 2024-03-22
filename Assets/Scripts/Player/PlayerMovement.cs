using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private int moveSpeed;
    [SerializeField] private float groundDrag;
    private float forwardInput;
    private float rightInput;
    private Vector3 moveDirection;
    [SerializeField] private float jumpForce;
    private bool readyToJump;


    [Header("")]
    [Header("Key Binds")]
    [SerializeField] private KeyCode jumpKey;


    private bool grounded;
    private Rigidbody rb;

    //---                                ---//

    void Start() 
    {
        rb = GetComponent<Rigidbody>();

        readyToJump = true;
    }

    void FixedUpdate()
    {
        GroundCheck();
        GetDirection();

        if(Input.GetKey(jumpKey))
            Jump();

        MovePlayer();
    }

    void GetDirection()
    {
        forwardInput = Input.GetAxisRaw("Vertical");
        rightInput = Input.GetAxisRaw("Horizontal");

        moveDirection = transform.forward * forwardInput + transform.right * rightInput;
    }

    void MovePlayer()
    {
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 620, ForceMode.Force);
        else
            rb.AddForce(-transform.up * 9.81f * 62, ForceMode.Force);
    }

    void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, 1.1f);

        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0.9f;
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
