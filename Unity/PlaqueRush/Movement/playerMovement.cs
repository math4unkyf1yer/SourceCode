using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed; //constant forward speed      
    public float testingSpeed;
    public float laneChangeSpeed = 10f; //how quickly we move sideway;
    public float maxHorizontalOffset = 3f; // Limit for left/right movement
    private float horizontalInput; // from mouse or keyboard
    private float basePosition;// use to recenter player whe nturnig and using the z axis 
    private bool isWaitingForTurnInput = false;
    private bool allowLeftTurn = false;
    private bool allowRightTurn = false;
    private float speedRot = 360f;                // deg/sec rotation speed
    private float nextBasePosition;
    private Transform platformTransform;

    public GameObject testting;

    //direction holder
    public enum CompassDir { North, East, South, West }
    public CompassDir currentDir = CompassDir.North;
    public bool invertInputs = false;

    //know if movement on x or z
    bool xActivated = true;
    bool turning;
    //rotation
    private Quaternion targetRotation;

    [Header("Jump / Gravity")]
    public float jumpForce = 12f;
    public float extraGravity = 20f;         //if not fly forever
    private float extraGravityClone;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;

    private Rigidbody rb;
    Vector3 targetPosition;

    [Header("Slide")]
    private bool sliding;
    public Animator playerAnimation;

    [Header("mobile control")]
    // Swipe detection
    private Vector2 startTouchPos;
    private Vector2 currentTouchPos;
    private bool stopTouch = false;

    [Header("Swipe Settings")]
    public float swipeRange = 50f; // Minimum distance in pixels to count as a swipe
    public float tapRange = 10f;   // To prevent tap from counting as a swipe




    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

       

       // playerAnimation = GetComponent<Animator>();
        // Start base position whichever axix needed
        basePosition = xActivated ? transform.position.x : transform.position.z;
    }

    private void Start()
    {
        SetMovement();
    }
    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        // Use mouse movement on PC
        float mouseX = (Input.mousePosition.x / (float)Screen.width) * 2f - 1f;
        horizontalInput = Mathf.Clamp(mouseX, -1f, 1f);
        if (invertInputs)
        {
            horizontalInput = -horizontalInput;
        }
#else
    // Use accelerometer on mobile
    Vector3 tilt = Input.acceleration;
    horizontalInput = Mathf.Clamp(tilt.x * 2f, -1f, 1f); // multiply to adjust sensitivity
#endif

        // A or D to turn. Can only happen when trigger collider at the choose point 
        if (isWaitingForTurnInput)
        {
            if (allowLeftTurn && Input.GetKeyDown(KeyCode.A))
            {
                currentDir = (CompassDir)(((int)currentDir + 3) % 4);
                StartTurn(-90f); // left
            }
            else if (allowRightTurn && Input.GetKeyDown(KeyCode.D))
            {
                currentDir = (CompassDir)(((int)currentDir + 1) % 4);
                StartTurn(90f); // right
            }

        }
        //check if player jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.S) && !sliding)
        {
            Slide();
        }

#if !UNITY_STANDALONE && !UNITY_EDITOR
    DetectSwipe();
#endif

    }

    private void FixedUpdate()
    {
        //extra grav 
        GiveExtraGrav();
        //moooovvvement
        Movement();
    }
    private void Movement()
    {
        //check if turning 
        if (turning)
        {
            // rotate smoothly toward target rotation while stopping movement
            Quaternion currentRotation = rb.rotation;
            Quaternion nextRotation = Quaternion.RotateTowards(currentRotation, targetRotation, speedRot * Time.fixedDeltaTime);
            rb.MoveRotation(nextRotation);

            // done rotating, we need to change movement base on the other axis. if x then z vise versa
            if (Quaternion.Angle(currentRotation, targetRotation) < 0.5f)
            {
                rb.MoveRotation(targetRotation);
                turning = false;

                // Toggle axis (we turn 90�, so axis flips)
                xActivated = !xActivated;

                // set the new base position, to center player 
                basePosition = nextBasePosition;

                // Snap the player axis 
                if (currentDir == CompassDir.North || currentDir == CompassDir.South)
                {
                    transform.position = new Vector3(basePosition, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, basePosition);
                }

                // Finish turning
                isWaitingForTurnInput = false;
                allowLeftTurn = allowRightTurn = false;
                horizontalInput = 0f;
            }

            // velocity 0 to turn 
            rb.velocity = Vector3.zero;
            return;
        }
        // Normal movement 
        Vector3 forwardDisp = transform.forward * forwardSpeed * Time.fixedDeltaTime;
        if(!turning)
        {
            // x or z movement
            if (currentDir == CompassDir.North || currentDir == CompassDir.South)
            {
                float unclampedTargetX = transform.position.x + horizontalInput * laneChangeSpeed * Time.fixedDeltaTime;
                float clampedTargetX = Mathf.Clamp(unclampedTargetX, basePosition - maxHorizontalOffset, basePosition + maxHorizontalOffset);
                float newX = Mathf.Lerp(transform.position.x, clampedTargetX, 0.2f);
                Vector3 targetPos = transform.position + forwardDisp;
                targetPos.x = newX;
                rb.MovePosition(targetPos);
            }
            else
            {
                float unclampedTargetZ = transform.position.z + horizontalInput * laneChangeSpeed * Time.fixedDeltaTime;
                float clampedTargetZ = Mathf.Clamp(unclampedTargetZ, basePosition - maxHorizontalOffset, basePosition + maxHorizontalOffset);
                float newZ = Mathf.Lerp(transform.position.z, clampedTargetZ, 0.2f);
                Vector3 targetPos = transform.position + forwardDisp;
                targetPos.z = newZ;
                rb.MovePosition(targetPos);
            }
        }
    }
    private void InvertInputs()
    {
        switch (currentDir)
        {
            case CompassDir.North: invertInputs = false; break;
            case CompassDir.East: invertInputs = true; break;
            case CompassDir.South: invertInputs = true; break;
            case CompassDir.West: invertInputs = false; break;
        }

    }
    private void Jump()
    {
        //Add force 
            playerAnimation.SetTrigger("Jumping");
            //make sure y 0 for safety 
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (sliding)
            {
                StoppingSlide();
            }
        
    }
    private void GiveExtraGrav()// make the jumping feel good  --- if not player fly 
    {
        if (!isGrounded())
        {
            // Debug.Log("Add extra force ");
            rb.AddForce(Vector3.down * extraGravity);
        }
        else
        {
            if(extraGravity != 20)
            {
                extraGravity = 20;
            }
        }
    }
    bool isGrounded()//check touch floor 
    {
         return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Slide()
    {
        
            if (!isGrounded())
            {
                extraGravityClone = extraGravity;
                extraGravity = extraGravity* 2;
            }
            sliding = true;
            playerAnimation.SetBool("Sliding", true);
            CollideSlide[] slides = FindObjectsOfType<CollideSlide>();
            foreach (var s in slides)
            {
                s.SetColliderActive(false);
            }
            StartCoroutine(SlideStop());
        
    }

    IEnumerator SlideStop()
    {
        yield return new WaitForSeconds(0.8f);
        StoppingSlide();
    }
    void StoppingSlide()
    {
        sliding = false;
        CollideSlide[] slides = FindObjectsOfType<CollideSlide>();
        foreach (var s in slides)
        {
            s.SetColliderActive(true);
        }
        playerAnimation.SetBool("Sliding", false);
    }
   
    // direction: "L" (left), "R" (right), "B" optional(left or right). Given at the start of new platform 
    public void TurnPlayerTemple(string direction, Transform center)
    {
        isWaitingForTurnInput = true;
        allowLeftTurn = (direction == "L" || direction == "B");
        allowRightTurn = (direction == "R" || direction == "B");
        platformTransform = center;
    }

    // Turning player 
    private void StartTurn(float turnAngle)
    {
        InvertInputs();
        // left or right location 
        Vector3 currentEuler = transform.eulerAngles;
        currentEuler.y += turnAngle;
        targetRotation = Quaternion.Euler(currentEuler);

        // new base position
        if (currentDir == CompassDir.East || currentDir == CompassDir.West)
        {
            nextBasePosition = platformTransform.position.z;
        }
        else
        {
            nextBasePosition = platformTransform.position.x;
        }

        turning = true;

        
        isWaitingForTurnInput = false;
        allowLeftTurn = allowRightTurn = false;
    }

    private void DetectSwipe()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPos = touch.position;
                stopTouch = false;
                break;

            case TouchPhase.Moved:
                currentTouchPos = touch.position;
                Vector2 delta = currentTouchPos - startTouchPos;

                if (stopTouch) return;

                float absX = Mathf.Abs(delta.x);
                float absY = Mathf.Abs(delta.y);

                // ──────────────────────────────────────────────
                // LEFT / RIGHT swipe
                // ──────────────────────────────────────────────
                if (absX > absY && absX > swipeRange && isWaitingForTurnInput)
                {
                    if (delta.x < 0)
                    {
                        // left swipe
                        if ( allowLeftTurn)
                        {
                            currentDir = (CompassDir)(((int)currentDir + 3) % 4);
                            StartTurn(-90f);
                        }
                    }
                    else
                    {
                        // right swipe
                        if (allowRightTurn)
                        {
                            currentDir = (CompassDir)(((int)currentDir + 1) % 4);
                            StartTurn(90f);
                        }
                    }

                    stopTouch = true;
                    break;
                }

                // ──────────────────────────────────────────────
                // UP / DOWN swipe
                // ──────────────────────────────────────────────
                if (absY > absX && absY > swipeRange)
                {
                    if (delta.y > 0)
                    {
                        // Up
                        Jump();
                    }
                    else
                    {
                        // Down
                        Slide();
                    }

                    stopTouch = true;
                    break;
                }

                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                stopTouch = true;
                break;
        }
    }

   public void SetIdle()
    {
        playerAnimation.SetBool("IsIdle", true);
    }
    public void SetMovement()
    {
        playerAnimation.SetBool("IsIdle", false);
    }

    //turning player mobile 
    private void DetectSwipeUp()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    stopTouch = false;
                    break;

                case TouchPhase.Moved:
                    currentTouchPos = touch.position;
                    Vector2 distance = currentTouchPos - startTouchPos;

                    if (!stopTouch)
                    {
                        // Vertical swipe detection
                        if (Mathf.Abs(distance.y) > swipeRange && Mathf.Abs(distance.x) < swipeRange / 2)
                        {
                            if (distance.y > 0)
                            {
                                // Swipe up -> jump
                                Jump();
                            }
                            else
                            {
                                // Swipe down -> slide
                                Slide();
                            }

                            stopTouch = true;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    stopTouch = true;
                    break;
            }
        }
    }



}
