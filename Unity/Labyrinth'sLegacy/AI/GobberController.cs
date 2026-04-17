using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GobberController : MonoBehaviour
{
    public float speed = 5f;
    private float chaseSpeed = 7f;
    public float chaseRange;
    public float attackRange;
    private float poopRange = 13f;
    private bool GobberStop;
    private bool chasingPlayer;
    // Length of the raycast
    public float sightDistance = 17f;
    public Transform target;
    //Gobber eyes 
    public GameObject eyes;
    [Header("AttackingGobber")]
    private int onlyOnce = 0; //make sure can only be do once 
    private bool canAttack = true;//make sure that gobber can attack 
    private bool isAttacking;
    private string WhattypeOfAttack;
    private int blundDamage = 13;
    public GameObject poop;
    public Transform poopLocation;
    private bool trowingPoop;
    private bool throwfirstPoop;
    [Header("Patrol")]
    // LayerMask to specify which layers to check for
    public LayerMask targetLayer;
    public Transform[] startPositions;
    private Rigidbody rb;
    private int currentPatrolIndex = 0; // Keeps track of the current patrol point
    public float patrolThreshold = 0.5f;
    [Header("Script")]
    private TerGen tergenChosen;
    private PlayerMovement playerMovementScript;
    private HitBox gobberHealth;
    private Stats statsScript;

    //check if on floor
    private bool isGrounded;
    //Animator 
    private Animator gobberAnimation;



    void Start()
    {
        gobberHealth = GetComponent<HitBox>();
        gobberAnimation = GetComponent<Animator>();
        target = GameObject.Find("Player").transform;
        playerMovementScript = target.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        // Initialize the array with 4 positions
        startPositions = new Transform[4];
        GetTerGen();
        // Create new GameObjects to hold the positions and assign them
        Scene loadedScene = SceneManager.GetSceneByName(tergenChosen.sceneName);//only use to make sure everything is in the right scene 
        for (int i = 0; i < startPositions.Length; i++)
        {
            startPositions[i] = new GameObject("StartPosition" + (i + 1)).transform;
            SceneManager.MoveGameObjectToScene(startPositions[i].gameObject, loadedScene);
            startPositions[i].SetParent(tergenChosen.parentForEnemyPosition);
        }
        // Calculate the positions dynamically
        float terrainHeight;
        Vector3[] offsets = {
         new Vector3(10, 0, 10),
         new Vector3(-10, 0, 10),
         new Vector3(-10, 0, -10),
         new Vector3(10, 0, -10)
       };

        for (int i = 0; i < startPositions.Length; i++)
        {
            // Determine the X and Z coordinates
            Vector3 targetPosition = this.transform.position + offsets[i];

            // Adjust Y based on terrain height
            Terrain terrain = Terrain.activeTerrain; // Get the active terrain
            if (terrain != null)
            {
                terrainHeight = terrain.SampleHeight(targetPosition);
            }
            else
            {
                terrainHeight = targetPosition.y; // Fallback in case there's no terrain
            }

            // Set the spawn position
            startPositions[i].position = new Vector3(targetPosition.x, terrainHeight, targetPosition.z);
        }
    }
    private void GetTerGen()
    {
        float closestDistance = float.MaxValue;
        TerGen[] terGens = FindObjectsOfType<TerGen>();
        foreach (TerGen terGen in terGens)
        {
            float distance = Vector3.Distance(this.gameObject.transform.position, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                tergenChosen = terGen;
            }
        }
    }
    void CheckGroundStatus()
    {
        // Perform a raycast slightly below the boar to check for the ground
        Ray ray = new Ray(transform.position, Vector3.down);
        float rayLength = 1.5f; // Adjust this based on the boar's height
        isGrounded = Physics.Raycast(ray, rayLength, LayerMask.GetMask("WhatisGround")); // Ensure the ground layer is correct
    }
    void FixedUpdate()
    {
        CheckGroundStatus();
        if (!isGrounded)
        {
            //maybe add something for better rotatidon 
            return;
        }

        // Define the starting point and direction of the ray
        Vector3 rayOrigin = eyes.transform.position;
        Vector3 rayDirection = eyes.transform.forward;
        // Cast the ray
        RaycastHit hit;
        //Add something for it to trow rock 
        if (target != null && GobberStop == false)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(gobberHealth.health <= 3)
            {
                if(trowingPoop == false)
                {
                    TrowPoop();
                }
                return;
            }
            if (distanceToPlayer <= attackRange && !isAttacking && canAttack)//attack
            {
                Attack();
            }
            if (distanceToPlayer <= chaseRange && !isAttacking && playerMovementScript.crouching == false || distanceToPlayer <= 20 && chasingPlayer && !isAttacking) // Only chase if the player is within range
            {
                ChasePlayer();
                chasingPlayer = true;
            }
            else if (Physics.Raycast(rayOrigin, rayDirection, out hit, sightDistance, targetLayer) && !isAttacking)
            {
                ChasePlayer();
            }
            else if (!isAttacking)
            {
                Patrol();//invisible square
                chasingPlayer = false;
            }
        }
    }

    void ChasePlayer()
    {
        if (!statsScript)
            statsScript = target.GetComponent<Stats>(); // tell player he is fighting
        if (!statsScript.fighting)
            statsScript.fighting = true;
        gobberAnimation.SetBool("IsRunningGober", true);
        gobberAnimation.SetBool("IsWalkingGobber", false);
        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Move the boar towards the target
        rb.MovePosition(transform.position + direction * chaseSpeed * Time.fixedDeltaTime);

        // Rotate the boar to face the target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * chaseSpeed));

        // Keep the boar upright by resetting the X and Z rotation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    void Patrol()
    {
        if (startPositions.Length == 0) return;

        if (statsScript != null)
        {
            if (statsScript.fighting)
                statsScript.fighting = false;
            statsScript = null;
        }

        gobberAnimation.SetBool("IsWalkingGobber", true);
        gobberAnimation.SetBool("IsRunningGober", false);
        // Get the next patrol point
        Transform patrolPoint = startPositions[currentPatrolIndex];

        // Calculate the direction to the patrol point, but ignore the y-axis
        Vector3 targetPosition = new Vector3(patrolPoint.position.x, transform.position.y, patrolPoint.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move towards the patrol point along the x and z axes only
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        // Rotate to face the patrol point, but ignore y-axis rotation
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));

        // Keep the boar upright by resetting the X and Z rotation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);

        // Check if the boar is close enough to the patrol point
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPosition.x, 0, targetPosition.z)) < patrolThreshold)
        {
            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % startPositions.Length;
        }
    }

    void TrowPoop()
    {
     //   if (trowingPoop) return; // Prevent multiple calls
        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        
        if (distanceToPlayer < poopRange && throwfirstPoop == false)
        {
            // Run away from the player if within poopRange
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            // Set the Gobber's animation to running
            gobberAnimation.SetBool("IsRunningGober", true);
            gobberAnimation.SetBool("IsWalkingGobber", false);

            // Move the Gobber in the opposite direction of the player
            rb.MovePosition(transform.position + directionAwayFromPlayer * speed * Time.deltaTime);

            // Rotate the Gobber to face the opposite direction
            Quaternion lookRotation = Quaternion.LookRotation(directionAwayFromPlayer);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.deltaTime * speed));

            // Keep the Gobber upright
            rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
        }
        else
        {
            throwfirstPoop = true;
            // Stop moving once outside poopRange
            gobberAnimation.SetBool("IsRunningGober", false);
            Vector3 directionToPlayer = (target.position - transform.position).normalized;
            Quaternion lookAtPlayer = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate towards the player until nearly aligned
            while (Quaternion.Angle(rb.rotation, lookAtPlayer) > 1f)
            {
                rb.rotation = Quaternion.Slerp(rb.rotation, lookAtPlayer, Time.deltaTime * speed * 2);
            }
            RotateAndThrowPoop();
        }
    }

    void RotateAndThrowPoop()
    {
        trowingPoop = true; // Prevent multiple throws
        // Calculate the direction to the player
        gobberAnimation.SetTrigger("ThrowPoop");
        // Optionally instantiate the poop object at Gobber's position
        GameObject poopp =  Instantiate(poop, poopLocation.transform.position, Quaternion.identity);
        // Pass the player’s position to the poop script to update its target
        PoopScript poopScript = poopp.GetComponent<PoopScript>();
        poopScript.SetTargetPosition(target.position);
        // Reset GobberStop after the animation completes, if needed
        StartCoroutine(ResumeAfterPoop());
    }

    IEnumerator ResumeAfterPoop()
    {
        // Wait for the animation to complete (adjust time as necessary)
        yield return new WaitForSeconds(2f);
        trowingPoop = false;
    }
    void Attack()
    {
        canAttack = false;
        gobberAnimation.SetBool("IsRunningGober", false);
        gobberAnimation.SetTrigger("IsAttacking");
        //bool is attacking
        isAttacking = true;
        WhattypeOfAttack = "blund";
        //play animation 
        //Ienumerator not attacking any more 
        StartCoroutine(attackStop());
    }
    IEnumerator attackStop()
    {
        yield return new WaitForSeconds(.5f);
        onlyOnce = 0;
        isAttacking = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj" && isAttacking && onlyOnce == 0)
        {
            Debug.Log("touch player");
            onlyOnce = 1;
            Stats statsScript = target.gameObject.GetComponent<Stats>();
            if (WhattypeOfAttack == "blund")
            {
                statsScript.TakebleugDamage(blundDamage);
            }
            Debug.Log("Touch Player");
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "OuterMap")
        {
            HitBox hit = this.gameObject.GetComponent<HitBox>();
            hit.dieByFallDamage = true;
            hit.SlashDamage(100, 0);
        }
    }

    void OnDrawGizmos()
    {
        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.1f);
    }
}
