using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement_3Person : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public Transform orientation;
    public float groundDrag;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    [Header("Jump")]
    public float jumpForce;
    public float jumpCooldown;
    public float jumpMultiplier;
    bool readyToJump;
    [Header("Crounching")]
    public float crouchSpeed;
    public float crouchYScale;
    float startYScale;
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitSlope;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;




    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,sprinting,air,crounching
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        setInputs();
        SpeedControl();
        StateHandler();

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    void StateHandler()
    {   //Mode - Crouch
        if(Input.GetKey(crouchKey))
        {
            state = MovementState.crounching;
            moveSpeed = crouchSpeed;
        }
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    void setInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //stop crouch
        if(Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

        }
    }

    void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if(OnSlope() && !exitSlope)
        {
            rb.AddForce(getSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if(rb.velocity.y > 0 )
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }
        //on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * jumpMultiplier, ForceMode.Force);
        //turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {

        //limit speed on slope
        if(OnSlope() && !exitSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            //limit speed on ground or in air
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

       
    }

    void Jump()
    {
        exitSlope = true;
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

     void ResetJump()
    {
        readyToJump = true;
        exitSlope = false;
    }
    
    bool OnSlope()
    {
        if(Physics.Raycast(transform.position,Vector3.down,out slopeHit,playerHeight * 0.5f + 0.3f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * slopeHit.distance, Color.yellow);
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    Vector3 getSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }


}
