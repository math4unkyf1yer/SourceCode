using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BoarController : MonoBehaviour
{
    public float speed = 5f;
    public float chaseRange = 15f;
    public float attackRange = 9f;
    public float dashSpeed = 25f;
    public float sightDistance = 17f;
    public Transform target;
    public Transform[] startPositions;
    //Boar eyes 
    public GameObject eyes;
    // LayerMask to specify which layers to check for
    public LayerMask targetLayer;
    private Rigidbody rb;
    private int currentPatrolIndex = 0; // Keeps track of the current patrol point
    public float patrolThreshold = 0.5f; // Distance threshold to decide when the boar has reached a patrol point
    private int slashDamage = 6;
    private int pierceDamage = 10;
    public bool boarStop;
    public bool isAttacking = false;
    private bool chasingPlayer;
    bool WaitForChase = false;
    bool WaitForReset = false;
    private string WhattypeOfAttack;
    private TerGen tergenChosen;
    private PlayerMovement playerMovementScript;
    private Stats statsScript;

    //make sure touches ground  
    private bool isGrounded;

    public GameObject dashEffect;

    [Header("Animation")]//Animation for the boar
    public Animator boarAnimation;

    void Start()
    {
        Vector3 correctedPosition = new Vector3(transform.position.x, 6, transform.position.z);
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
    void CheckGroundStatus()
    {
        // Perform a raycast slightly below the boar to check for the ground
        Ray ray = new Ray(transform.position, Vector3.down);
        float rayLength = 1.5f; // Adjust this based on the boar's height
        isGrounded = Physics.Raycast(ray, rayLength, LayerMask.GetMask("WhatisGround")); // Ensure the ground layer is correct
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

    void FixedUpdate()
    {
        CheckGroundStatus();
        if (!isGrounded) {
            //maybe add something for better rotatidon 
            return;
        } 
        // Define the starting point and direction of the ray
        Vector3 rayOrigin = eyes.transform.position;
        Vector3 rayDirection = eyes.transform.forward;
        // Cast the ray
        RaycastHit hit;
        if (target != null && boarStop == false)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartCoroutine(AttackDash());
            }
            if (distanceToPlayer <= chaseRange && !isAttacking && WaitForChase == false && playerMovementScript.crouching == false || distanceToPlayer <= 20 &&chasingPlayer && !isAttacking && WaitForChase == false) // Only chase if the player is within range
            {
                boarWalkingAnim();//anim
                ChasePlayer();
                chasingPlayer = true;
            }
            else if (Physics.Raycast(rayOrigin, rayDirection, out hit, sightDistance, targetLayer) && !isAttacking )
            {
                boarWalkingAnim();
                ChasePlayer();
            }
            else if(!isAttacking && WaitForChase ==  false)
            {
                boarWalkingAnim();
                Patrol();//invisible square
                chasingPlayer = false;
            }
        }
    }

    void boarWalkingAnim()
    {
        if (boarAnimation.GetBool("Walking") == false)
        {
            boarAnimation.SetBool("Walking", true);
            boarAnimation.SetBool("Running", false);
        }
    }
    void boarDashAnim()
    {
        boarAnimation.SetBool("Walking", false);
        boarAnimation.SetBool("Running", true);
    }

    void ChasePlayer()
    {
        if (!isGrounded) return;

        if (!statsScript)
            statsScript = target.GetComponent<Stats>(); // tell player he is fighting
        if (!statsScript.fighting)
            statsScript.fighting = true;
        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Move the boar towards the target
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        // Rotate the boar to face the target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));

        // Keep the boar upright by resetting the X and Z rotation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    //dash Attack Pierce Damage
    IEnumerator AttackDash()
    {
        if (WaitForReset == false)
        {
            dashEffect.SetActive(true);//effect of boar 
            WhattypeOfAttack = "Pierce";
            WaitForChase = true;
            WaitForReset = true;
            isAttacking = true;
            yield return new WaitForSeconds(0.2f);
            boarDashAnim();
            // Store the player's position at the moment the attack starts
            Vector3 targetPosition = target.position;

            Vector3 dashDirection = (targetPosition - transform.position).normalized;

            // **Overshoot past the player** by adding extra distance (e.g., +3f)
            float overshootDistance = 5f; // Adjust this to change overshoot effect
            Vector3 dashTargetPosition = targetPosition + (dashDirection * overshootDistance);

            float leapDuration = (8f + overshootDistance) / dashSpeed; // Adjusted for overshoot

            float leapTimer = 0f;
            while (leapTimer < leapDuration)
            {

                // Smoothly move the boar towards the target position
                rb.MovePosition(Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.fixedDeltaTime));

                // Keep the boar upright
                rb.rotation = Quaternion.LookRotation(dashDirection);

                leapTimer += Time.fixedDeltaTime;
                yield return null;
            }
            // Ensure the boar has reached the target before allowing another attack
            while (Vector3.Distance(transform.position, dashTargetPosition) > .5f)
            {
                // Move the boar towards the target position
                rb.MovePosition(Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.fixedDeltaTime));

                yield return null;
            }

            // Stop the boar once it reaches the target
            rb.velocity = Vector3.zero;

            yield return new WaitForSeconds(.1f);
            isAttacking = false;
            yield return new WaitForSeconds(1f);
            WaitForChase = false;
            dashEffect.SetActive(false);
            // Optionally, add a delay before allowing another attack
            yield return new WaitForSeconds(2f);
            WaitForReset = false;
        }
       
    }
    IEnumerator SwipAttack()
    {
        if (WaitForReset == false)
        {
            WhattypeOfAttack = "Slash";
            WaitForChase = true;
            WaitForReset = true;
            isAttacking = true;
            boarAnimation.SetTrigger("Attack"); // Assuming an animation for slash attack

            yield return new WaitForSeconds(0.3f); // Small wind-up before attacking

            // Deal damage in front of the boar
            Vector3 attackPosition = transform.position + transform.forward * 2f; // Adjust distance as needed
            Collider[] hitObjects = Physics.OverlapSphere(attackPosition, 2f, targetLayer); // Check if player is in range

            foreach (Collider hit in hitObjects)
            {
                if (hit.CompareTag("Player"))
                {
                    Stats playerStats = hit.GetComponent<Stats>();
                    if (playerStats)
                    {
                        playerStats.TakeSlashDamage(slashDamage);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f); // Time before boar can act again
            isAttacking = false;
            yield return new WaitForSeconds(1f);
            WaitForChase = false;
            yield return new WaitForSeconds(3f);
            WaitForReset = false;
        }
    }
    void Patrol()
    {
        if (!isGrounded) return;

        if (startPositions.Length == 0) return;

        if (statsScript != null)
        {
            if (statsScript.fighting)
                statsScript.fighting = false;
            statsScript = null;
        }

        // Get the next patrol point
        Transform patrolPoint = startPositions[currentPatrolIndex];

        // Calculate the direction to the patrol point on the XZ plane
        Vector3 targetPosition = new Vector3(patrolPoint.position.x, patrolPoint.position.y, patrolPoint.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Maintain gravity
        Vector3 currentVelocity = rb.velocity;
        Vector3 horizontalVelocity = new Vector3(direction.x * speed, currentVelocity.y, direction.z * speed);

        // Apply the velocity
        rb.velocity = horizontalVelocity;

        // Rotate the boar to face the patrol point
        if (horizontalVelocity.magnitude > 0.1f) // Only rotate if the boar is moving
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
        }

        // Check if the boar is close enough to the patrol point
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPosition.x, 0, targetPosition.z)) < patrolThreshold)
        {
            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % startPositions.Length;
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player" && isAttacking)
        {
            Stats statsScript = collision.gameObject.GetComponent<Stats>();
            if(WhattypeOfAttack == "Pierce")
            {
                statsScript.TakepierceDamage(pierceDamage);
            }else if(WhattypeOfAttack == "Slash")
            {

            }
            isAttacking = false;
        }
        if(collision.gameObject.tag == "OuterMap")
        {
            HitBox hit = this.gameObject.GetComponent<HitBox>();
            hit.dieByFallDamage = true;
            hit.SlashDamage(100, 0);
        }
    }
}


