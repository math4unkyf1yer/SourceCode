using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    private Transform player;
    public float radius = 10f;
    private float toFar = 50f;
    public float moveSpeed = 4f;
    public float rotationSpeed = 5f;

    private void Start()
    {
        GameObject playerObject = GameObject.Find("PlayerFollow");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("PlayerFollow not found!");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        RotateTowardPlayer();
        if (distance >= radius)
        {
            FollowPlayerLogic();
        }
    }

    private void RotateTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // Smoothly rotate toward the player
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void FollowPlayerLogic()
    {
        // Follow the player at eye-level (adjust y if needed)
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
