using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("References")]
    public Transform orientation; // Assign an empty GameObject for movement direction

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    public float runSpeedMultiplier = 1.7f; // Speed multiplier when running
    public float maxStamina = 100f; // Max stamina value
    public float staminaDrainRate = 17f; // How much stamina is drained per second while running
    public float staminaRegenRate = 3.5f; // How much stamina regenerates per second when not running
    public float staminaCooldownTimer = 3f; // Cooldown time before stamina can regenerate
    private float currentStamina; // Current stamina value
    private Vector3 moveDirection;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float playerHeight = 2f;
    private bool isjumping;
    private bool canResetJump = true;
    public LayerMask groundMask;
    public LayerMask moveObjectMask;
    public Transform groundCheck;

    private bool grounded;

    private Slider staminaSlider;

    public Selecting selectingScript;
    private PlatformMove platScript;

    public bool running;

    //for platforms 
    private PlatformMove currentPlatform = null;
    public float rayLength = 1.1f;

    [Header("Animation")]
    public Animator playerAnimation;
    void Start()
    {
        //   staminaSlider = GameObject.Find("StaminaSlider").GetComponent<Slider>();
        //   staminaSlider.maxValue = maxStamina;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotation
        currentStamina = maxStamina; // Initialize stamina
        staminaCooldownTimer = 0f; // Initialize the cooldown timer

    }

    [System.Obsolete]
    void Update()
    {
        MovementAnimation();
        GetInput();
        Jump();
        if(Time.timeScale == 0f && selectingScript.freezeActivated)
        {
            FreezeMovePlayer();
            if (!Physics.autoSimulation)
            {
                Physics.Simulate(Time.unscaledDeltaTime);
            }
        }
        else
        {
            MovePlayer();
        }
        //   UpdateStaminaSlider();
        //  HandleStamina();
    }

    void FixedUpdate()
    {
        IsPlayerOnPlat();
    }

    void MovementAnimation()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = horizontalVelocity.magnitude;
        playerAnimation.SetFloat("speed", speed);
        playerAnimation.SetBool("Grounded", isGrounded());
    }
    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        running = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton8)) && currentStamina > 0f;
    }
    private void FreezeMovePlayer()
    {
        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0;

        float speed = running ? moveSpeed * runSpeedMultiplier : moveSpeed;

        // Directly control velocity like normal movement
        Vector3 targetVelocity = moveDirection.normalized * speed;
        rb.velocity = targetVelocity + new Vector3(0, rb.velocity.y, 0);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        moveDirection.y = 0; // Prevent vertical movement

        float currentSpeed = running ? moveSpeed * runSpeedMultiplier : moveSpeed;

        Vector3 targetVelocity = moveDirection.normalized * currentSpeed;
        rb.velocity = targetVelocity + new Vector3(0, rb.velocity.y, 0);
        /*trying with transforms
        Vector3 velocity = moveDirection.normalized * currentSpeed;


        // Move manually during freeze
        if (Time.timeScale == 0)
        {
            transform.position += velocity * Time.unscaledDeltaTime;
           rb.MovePosition(rb.position + targetVelocity * Time.unscaledDeltaTime);
        }*/


    }

    private void Jump()
    {
        if (isGrounded() && isjumping == true && canResetJump)
        {
            playerAnimation.SetBool("JumpCheck", false);
            isjumping = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded() && selectingScript.isGrabbing == false)
            {
                isjumping = true;
                playerAnimation.SetTrigger("Jump");
                playerAnimation.SetBool("JumpCheck", true);
                canResetJump = false; // block reset right after takeoff
                StartCoroutine(AllowJumpReset()); // wait before allowing reset
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset vertical velocity
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Not grounded");
            }
        }
    }
    private IEnumerator AllowJumpReset()
    {
        yield return new WaitForSeconds(1.2f); // wait before ground check can reset isjumping
        canResetJump = true;
    }

    private bool isGrounded()
    {
        float checkDistance = 0.5f;
        Vector3[] rayOffsets = new Vector3[] // make sure you touch something 
        {
        Vector3.zero, // center
        new Vector3(0.33f, 0, 0.33f),   // front-right
        new Vector3(-0.33f, 0, 0.33f),  // front-left
        new Vector3(0.33f, 0, -0.33f),  // back-right
        new Vector3(-0.33f, 0, -0.33f)  // back-left
        };

        foreach (var offset in rayOffsets)
        {
            if (Physics.Raycast(groundCheck.position + offset, Vector3.down, checkDistance, groundMask))
            {
                return true;
            }
        }

        return false;
    }

    void IsPlayerOnPlat()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayLength))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.GetComponent<PlatformMove>() != null)
            {
                PlatformMove platform = hitObject.GetComponent<PlatformMove>();

                if (platform != null && platform.platG == false)
                {
                    // If we have a new platform detected
                    if (currentPlatform != platform)
                    {
                        // If previously on another platform, notify it we left
                        if (currentPlatform != null)
                            currentPlatform.PlayerOffObject();

                        currentPlatform = platform;
                        selectingScript.objectOn = hitObject;
                        currentPlatform.PlayerOnObject();
                    }
                    // else still on same platform, do nothing
                }
            }
            else
            {
                // Hit something, but not platform
                ClearCurrentPlatform();
            }
        }
        else
        {
            // Nothing detected below player within rayLength
            ClearCurrentPlatform();
        }
    }
    private void ClearCurrentPlatform()
    {
        if (currentPlatform != null)
        {
            currentPlatform.PlayerOffObject();
            currentPlatform = null;
            selectingScript.objectOn = null;
        }
    }
    /*  private void OnCollisionEnter(Collision collision)
      {
          if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
          {
              Debug.Log("asdsds");
              selectingScript.cannotGrabble = true;
              if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "PlatformG")
              {
                  platScript = collision.gameObject.GetComponent<PlatformMove>();
                  platScript.PlayerOnObject();
              }
          }
      }
      private void OnCollisionExit(Collision collision)
      {
          if (collision.gameObject.layer == 6 || collision.gameObject.layer == 7)
          {
              Debug.Log("Exit");
              selectingScript.cannotGrabble = false;
              if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "PlatformG")
              {
                  platScript = collision.gameObject.GetComponent<PlatformMove>();
                  platScript.PlayerOffObject();
              }
          }
      }*/

    /* private void HandleStamina()
     {
         // If stamina is above 0, drain stamina when running
         if (running)
         {
             currentStamina -= staminaDrainRate * Time.deltaTime;
             if (currentStamina < 0)
             {
                 currentStamina = 0;
                 staminaCooldownTimer = 3; // Start cooldown when stamina reaches zero
                 moveSpeed = 2;
             }
         }
         else
         {
             // If the cooldown timer is running, do not regenerate stamina
             if (staminaCooldownTimer > 0f)
             {

                 staminaCooldownTimer -= Time.deltaTime;
             }
             else
             {

                 if (moveSpeed != 5) moveSpeed = 5;
                 // Regenerate stamina when not running and cooldown has passed
                 currentStamina += staminaRegenRate * Time.deltaTime;
                 if (currentStamina > maxStamina) currentStamina = maxStamina;
             }
         }
     }
     private void UpdateStaminaSlider()
     {
         if (staminaSlider != null)
         {
             // Update the stamina slider to reflect current stamina value
             staminaSlider.value = currentStamina;
         }
     }*/
}


