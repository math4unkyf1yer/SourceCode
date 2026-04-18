using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class EnemyBaseController : MonoBehaviour
{
    public float speed = 5f;
    public float sightDistance = 17f;
    public Transform target;
    public GameObject eyes;
    public LayerMask targetLayer;
    public Transform[] startPositions;

    protected Rigidbody rb;
    protected int currentPatrolIndex = 0;
    public float patrolThreshold = 0.5f;
    protected bool isGrounded;
    protected TerGen tergenChosen;
    protected PlayerMovement playerMovementScript;
    protected Stats statsScript;

    protected virtual float GroundCheckLength => 1.5f;
    protected virtual float ChaseSpeed => speed;

    protected virtual void Start()
    {
        target = GameObject.Find("Player").transform;
        playerMovementScript = target.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        GetTerGen();
    }

    protected void SetupPatrolPoints()
    {
        startPositions = new Transform[4];
        Scene loadedScene = SceneManager.GetSceneByName(tergenChosen.sceneName);
        for (int i = 0; i < startPositions.Length; i++)
        {
            startPositions[i] = new GameObject("StartPosition" + (i + 1)).transform;
            SceneManager.MoveGameObjectToScene(startPositions[i].gameObject, loadedScene);
            startPositions[i].SetParent(tergenChosen.parentForEnemyPosition);
        }

        Vector3[] offsets = {
            new Vector3(10, 0, 10),
            new Vector3(-10, 0, 10),
            new Vector3(-10, 0, -10),
            new Vector3(10, 0, -10)
        };

        Terrain terrain = Terrain.activeTerrain;
        for (int i = 0; i < startPositions.Length; i++)
        {
            Vector3 pos = transform.position + offsets[i];
            float y = terrain != null ? terrain.SampleHeight(pos) : pos.y;
            startPositions[i].position = new Vector3(pos.x, y, pos.z);
        }
    }

    protected void GetTerGen()
    {
        float closestDistance = float.MaxValue;
        foreach (TerGen terGen in FindObjectsOfType<TerGen>())
        {
            float distance = Vector3.Distance(transform.position, terGen.parent.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                tergenChosen = terGen;
            }
        }
    }

    protected void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(new Ray(transform.position, Vector3.down), GroundCheckLength, LayerMask.GetMask("WhatisGround"));
    }

    protected virtual void Patrol()
    {
        if (!isGrounded || startPositions.Length == 0) return;

        if (statsScript != null)
        {
            statsScript.fighting = false;
            statsScript = null;
        }

        Transform patrolPoint = startPositions[currentPatrolIndex];
        Vector3 targetPos = new Vector3(patrolPoint.position.x, transform.position.y, patrolPoint.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPos.x, 0, targetPos.z)) < patrolThreshold)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % startPositions.Length;
        }
    }

    protected virtual void ChasePlayer()
    {
        if (!statsScript)
            statsScript = target.GetComponent<Stats>();
        if (!statsScript.fighting)
            statsScript.fighting = true;

        Vector3 direction = (target.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * ChaseSpeed * Time.fixedDeltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * ChaseSpeed));
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    protected void HandleOuterMapCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("OuterMap"))
        {
            HitBox hit = GetComponent<HitBox>();
            hit.dieByFallDamage = true;
            hit.SlashDamage(100, 0);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.1f);
    }
}
