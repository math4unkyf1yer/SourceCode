using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoppMovement : MonoBehaviour
{
    //patrols
    public Transform[] patrolPoints;
    private int currentPatrolIndex;

    public GameObject player;
    private NavMeshAgent copp;

    //copp view
    public float viewAngle = 70f;
    public float viewDistance = 20f;

    [Header("Laser Settings")]
    private LineRenderer laserLine;
    public GameObject eye;
    public float laserDuration = 0.2f;  // How long the laser is visible
    public float laserSpeed = 50f;      // How fast the laser "shoots"
    private bool firingLaser;
    private bool cooldownInProgress;

    private RespawnPoint respawnScript;
    private bool foundPlayer;
    public GameObject shootingEffect;

    //Audio
    private AudioSource coppAudio;
    public AudioClip alarm;
    public AudioClip laserShoot;
    // Start is called before the first frame update
    void Start()
    {
        coppAudio = GetComponent<AudioSource>();
        laserLine = GetComponent<LineRenderer>();
      //  player = GameObject.Find("Player").GetComponent<GameObject>();
        copp = GetComponent<NavMeshAgent>();
        respawnScript = GameObject.Find("GameManager").GetComponent<RespawnPoint>();
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        bool canSee = CanSeePlayer();
        HeadSpinning();

        if (canSee)
        {
            copp.isStopped = true;
            Vector3 lookDirection = player.transform.position - transform.position;
            lookDirection.y = 0;
            coppAudio.clip = alarm;
            if (!coppAudio.isPlaying && firingLaser == false)
            {
                coppAudio.Play();
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 10f);

            if (!firingLaser && !cooldownInProgress)
            {
                Debug.Log("will fire laser");
                StartCoroutine(FireLaser());
            }
        }
        else if (!firingLaser)
        {
            if (!copp.pathPending && copp.remainingDistance < 0.9f)
                Patrol();
        }
    }

    void HeadSpinning()
    {

    }
    IEnumerator FireLaser()
    {
        firingLaser = true;
        cooldownInProgress = true;
        yield return new WaitForSeconds(0.35f);
        shootingEffect.SetActive(true);
        // Step 5: only shoot if player still visible
        if (!CanSeePlayer())
        {
            shootingEffect.SetActive(false);
            // Player moved out of sight — cancel shot
            firingLaser = false;
            copp.isStopped = false;
            cooldownInProgress = false;
            yield break;
        }
        yield return new WaitForSeconds(0.2f);
        shootingEffect.SetActive(false);
        Vector3 oldPlayerPos = player.transform.position + Vector3.up * 0.7f;
        if (!CanSeePlayer())
        {
            // Player moved out of sight — cancel shot
            firingLaser = false;
            copp.isStopped = false;
            cooldownInProgress = false;
            yield break;
        }
        if (laserLine != null)
        {
            laserLine.enabled = true;

            PlayAudio();

            Vector3 startPos = eye.transform.position;
           
            Vector3 dir = (oldPlayerPos) - startPos;

            // Raycast to see what the laser hits
            if (Physics.Raycast(startPos, dir.normalized, out RaycastHit hit, viewDistance))
            {
                // Laser end point
                laserLine.SetPosition(0, startPos);
                laserLine.SetPosition(1, hit.point);

                // If it hits the player
                if (hit.collider.CompareTag("Player"))
                {
                    yield return new WaitForSeconds(laserDuration);
                    respawnScript.Respawn();
                }
            }
        }

        yield return new WaitForSeconds(laserDuration);

        if (laserLine != null) laserLine.enabled = false;
        copp.isStopped = false;
        firingLaser = false;
        yield return new WaitForSeconds(1f);
        cooldownInProgress = false;
    }

    void PlayAudio()
    {
        coppAudio.clip = laserShoot;
        coppAudio.Play();
    }
  /*  IEnumerator CooldownWait()
    {

    }*/
    IEnumerator playerResets()
    {
        yield return new WaitForSeconds(0.5f);
        respawnScript.Respawn();

        //continue Moving 
        copp.isStopped = false;
    }
    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < viewAngle / 2f)
        {
            Ray ray = new Ray(transform.position + Vector3.up, directionToPlayer.normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    foundPlayer = true;
                    return true;
                }
            }
        }

        return false;
    }


    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        copp.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Draw view range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Draw view angle lines
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rightBoundary * viewDistance);
    }
}
