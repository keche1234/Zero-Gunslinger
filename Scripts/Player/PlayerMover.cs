using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Parameters")]
    [SerializeField] private float speed;
    private float acceleration;
    [SerializeField] private float jumpHeight;
    private float jumpSpeed;

    // Internal Movement Properties
    private bool facingRight = true;
    private bool airborne = false;
    private Vector3 groundNormal; //updated when you make contact with a surface on the ground
    private float maxSlope;

    // Tracking ground position and gravity for respawn
    private Vector3 lastGroundPos;
    private Vector3 lastGravity;

    // Components
    private Rigidbody playerRb;
    private PlayerKeyManager playerKM;

    void Start()
    {
        if (speed == 0)
            Debug.LogWarning(this + ": Speed is 0! You can't move!");
        acceleration = speed * 20f;

        if (jumpHeight == 0)
            Debug.LogWarning(this + ": Jump Height is 0! You can't jump!");
        jumpSpeed = Mathf.Sqrt(2 * Physics.gravity.magnitude * jumpHeight);

        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null)
            Debug.LogError(this + ": No rigidbody on the player!");

        playerKM = GetComponent<PlayerKeyManager>();
        if (playerKM == null)
            Debug.LogError(this + ": No key manager on the player!");

        //facingRight = true;
        maxSlope = 46.001f;
        lastGravity = Physics.gravity;
    }

    void Update()
    {
        // Draw Gravity
        Debug.DrawRay(transform.position, Physics.gravity, Color.blue);
    }

    void FixedUpdate()
    {
        // Ground Detection (initial position is a little higher so the trace doesn't start in the ground)
        RaycastHit hit;
        if (Physics.SphereCast(transform.position + (transform.up * 0.2f), 0.5f, -transform.up, out hit, 0.5f, 1 << LayerMask.NameToLayer("Ground"))
            && Vector3.Angle(hit.normal, -Physics.gravity) <= maxSlope) // On ground
        {
            if (airborne == true)
                playerKM.TransferTemp();

            groundNormal = hit.normal;
            airborne = false;

            lastGroundPos = hit.point;
            lastGravity = Physics.gravity;
        }
        else // In the air
        {
            airborne = true;
            groundNormal = Vector3.zero;
        }

        //Rotate towards correct gravity
        if (Vector3.Angle(-transform.up, Physics.gravity) > 0.01f)
        {
            Vector3 downVector = Vector3.RotateTowards(-transform.up, Physics.gravity, Mathf.PI * 2 * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, 0, 90) * downVector, -downVector);
        }
    }

    public bool FacingRight()
    {
        return facingRight;
    }

    /*
     * Moves the player left or right based on horizontal input,
     * or slows them down.
     */
    public void Move(float input)
    {
        if (input != 0)
        {
            Vector3 moveDirection;
            Vector3 straightDirection = Quaternion.Euler(0, 0, 90 * input / Mathf.Abs(input)) * -transform.up;
            float speedCap = 1;
            bool walled = false;
            RaycastHit hit;
            if (groundNormal.magnitude == 0) // In the air
            {
                moveDirection = straightDirection;
                // Check if I will hit a wall 
                Debug.DrawRay(transform.position - (moveDirection * 0.5f), moveDirection * 1.5f, Color.magenta);
                if (Physics.SphereCast(transform.position - (moveDirection * 0.5f), 0.5f, moveDirection, out hit, 0.51f, 1 << LayerMask.NameToLayer("Ground"))) // On ground
                {
                    speedCap = Mathf.Clamp01(maxSlope - (Vector3.Angle(moveDirection, hit.normal) - 90f));
                    if (speedCap > 0f)
                        playerRb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
                    else
                        walled = true;
                }
                else
                    playerRb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
            }
            else
            {
                moveDirection = Quaternion.Euler(0, 0, -90 * input / Mathf.Abs(input)) * groundNormal;
                playerRb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
            }

            Vector3 horizontalVelocity = walled ? Vector3.zero : Vector3.Project(playerRb.velocity, moveDirection);
            Vector3 verticalSpeed = Vector3.Project(playerRb.velocity, Quaternion.Euler(0, 0, 90) * moveDirection);
            if (horizontalVelocity.magnitude > speed * speedCap)
                horizontalVelocity *= speed * speedCap / horizontalVelocity.magnitude;

            playerRb.velocity = horizontalVelocity + verticalSpeed;

            //Is there a turn around?
            if (Vector3.Angle(horizontalVelocity, Vector3.Project(transform.forward, moveDirection)) == 180)
            {
                transform.Rotate(0, 180, 0);
                facingRight = !facingRight;
            }
        }
        else
        {
            Vector3 slowDirection;
            //if (airborne)
            //{ // In the air
            //    slowDirection = -transform.forward;
            //}
            //else
            slowDirection = -Vector3.Project(playerRb.velocity, Quaternion.Euler(0, 0, 90 * (facingRight ? 1 : -1)) * groundNormal);
            slowDirection = slowDirection.normalized;

            Vector3 horizontalVelocity = Vector3.Project(playerRb.velocity, -slowDirection);

            //Stop deceleration after properly slowed down
            if (horizontalVelocity.magnitude > 0.01f && horizontalVelocity.normalized == transform.forward)
            {
                playerRb.AddForce(slowDirection * acceleration * (airborne ? 0.2f : 1), ForceMode.Acceleration);
            }
            else
            {
                playerRb.velocity = Vector3.Project(playerRb.velocity, (airborne ? transform.up : groundNormal));
            }
        }
    }

    /*
     * Atttempts to jump. If the player is airborne, the jump will fail
     * Returns the success of the jump.
     */
    public bool Jump()
    {
        if (!airborne)
        {
            playerRb.AddForce(groundNormal * jumpSpeed, ForceMode.VelocityChange);
            airborne = true;
            return true;
        }
        return false;
    }

    [ContextMenu("Respawn")]
    public void Respawn()
    {
        Physics.gravity = lastGravity;
        playerRb.position = lastGroundPos - (Physics.gravity.normalized * jumpHeight);
        playerRb.velocity = Vector3.zero;
        playerKM.DiscardTemp();
    }

    ///*
    // * Returns the last safe ground position
    // */
    //public Vector3 LastSafeGroundPosition()
    //{
    //    return lastGroundPos;
    //}

    ///*
    // * Returns the gravity of the last time you were on safe ground
    // */
    //public Vector3 LastSafeGravity()
    //{
    //    return lastGravity;
    //}
}
