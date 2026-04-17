using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeerController : MonoBehaviour
{
    public float speed = 5f;
    public int escapeRange = 7;
    public int safeDistance = 55;
    public float escapeSpeed = 10f;
    // Length of the raycast
    public float sightDistance = 17f;
    public Transform target;
    //deer eyes 
    public GameObject eyes;
    // LayerMask to specify which layers to check for
    public LayerMask targetLayer;
    public Transform[] startPositions;
    private Rigidbody rb;
    private int currentPatrolIndex = 0; // Keeps track of the current patrol point
    public float patrolThreshold = 0.5f;
    private bool escaping;
    private TerGen tergenChosen;
    private PlayerMovement playerMovementScript;

    //touch the ground
    private bool isGrounded;

    //Animation 
    public Animator deerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        playerMovementScript = target.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        startPositions = new Transform[4];
        GetTerGen();
        Scene loadedScene = SceneManager.GetSceneByName(tergenChosen.sceneName);
        // Create new GameObjects to hold the positions and assign them
        for (int i = 0; i < startPositions.Length; i++)
        {
            startPositions[i] = new GameObject("StartPositionDeer" + (i + 1)).transform;
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
        float rayLength = 1.9f; // Adjust this based on the boar's height
        isGrounded = Physics.Raycast(ray, rayLength, LayerMask.GetMask("WhatisGround")); // Ensure the ground layer is correct
    }

    // Update is called once per frame
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
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if(distanceToPlayer <= escapeRange && playerMovementScript.crouching == false || escaping == true)
        {
            Escape();
            if(distanceToPlayer > safeDistance)
            {
                escaping = false;
            }
        }else if (Physics.Raycast(rayOrigin, rayDirection, out hit, sightDistance, targetLayer))
        {
            Debug.Log("Hit Something");
            Escape();
        }
        else if(escaping == false)
        {
            Patrol();
        }
    }
    void Escape()
    {
        deerAnimator.SetBool("isRunning", true);
        deerAnimator.SetBool("isWalking", false);
        escaping = true;
        // Calculate the direction from the deer to the target
        Vector3 directionToTarget = (target.position - transform.position).normalized;

        // Reverse the direction to get the escape direction
        Vector3 escapeDirection = -directionToTarget;

        // Move the deer in the escape direction
        rb.MovePosition(transform.position + escapeDirection * escapeSpeed * Time.fixedDeltaTime);

        // Rotate the deer to face the escape direction
        Quaternion lookRotation = Quaternion.LookRotation(escapeDirection);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));

        // Keep the deer upright by resetting the X and Z rotation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    void Patrol()
    {
        deerAnimator.SetBool("isWalking", true);
        deerAnimator.SetBool("isRunning", false);
        if (startPositions.Length == 0) return;

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
