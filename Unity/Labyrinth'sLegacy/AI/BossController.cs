using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float speed = 5f;
    public float chaseRange = 20f;
    public float attackRange = 3f;
    public float dashSpeed = 2f;
    public float dashCooldown;
    private bool dashReady = true;
    public Transform target;
    // LayerMask to specify which layers to check for
    public LayerMask targetLayer;
    private Rigidbody rb;
    private int slashDamage = 8;
    private int pierceDamage = 15;
    //turning on blade colliders
    public BoxCollider[] bladeColliders;
    public bool isAttacking = false;
    private string WhattypeOfAttack;
    private TerGen tergenChosen;
    private PlayerMovement playerMovementScript;
    private Stats statsScript;

    public Animator BossAnimation;
    public GameObject DashEffect;

    //Audio
    private AudioSource bossSound;
    public AudioClip dashSound;

    private void Start()
    {
        bossSound = gameObject.GetComponent<AudioSource>();
        target = GameObject.Find("Player").transform;
        playerMovementScript = target.GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        GetTerGen();
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

    private void FixedUpdate()
    {
        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if(distanceToPlayer <= attackRange && !isAttacking )
            {
                StartCoroutine(SwipeAttack());
            }
            if (distanceToPlayer >= 8 && distanceToPlayer < 13 && !isAttacking && dashReady)
            {
               StartCoroutine(AttackDash());
            }
            if (distanceToPlayer <= chaseRange && !isAttacking ) // Only chase if the player is within range
            {
                ChasePlayer();
            }
            else
            {
                if(statsScript != null)
                {
                    if (statsScript.fighting)
                        statsScript.fighting = false;
                    statsScript = null;
                }
            }
        }
    }

    IEnumerator AttackDash()
    {
        bossSound.clip = dashSound;
        bossSound.Play();
        DashEffect.SetActive(true);
        dashReady = false;
        // Set the attack state and prevent other actions
        WhattypeOfAttack = "Pierce"; // Set the attack type
        isAttacking = true;          // Prevent other actions during the dash
        BossAnimation.SetBool("IsWalking", false); // Stop walking animation
        BossAnimation.SetTrigger("AttackDash");   // Play dash attack animation
        bladeColliders[0].enabled = true;
        bladeColliders[1].enabled = true;

        // Wait briefly before starting the dash
        yield return new WaitForSeconds(0.6f);

        // Capture the player's position at the start of the dash
        Vector3 targetPosition = target.position;

        // Calculate the direction to the target
        Vector3 dashDirection = (targetPosition - transform.position).normalized;

        // Calculate the dash duration based on distance and speed
        float dashDistance = Vector3.Distance(transform.position, targetPosition);
        float dashDuration = dashDistance / dashSpeed;

        // Move the boss toward the target position over time
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.fixedDeltaTime));
            elapsedTime += Time.fixedDeltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the boss stops at the target position
        rb.MovePosition(targetPosition);

        // Stop the dash and allow other actions after a delay
        yield return new WaitForSeconds(0.5f); // Add a cooldown after the dash
        bladeColliders[0].enabled = false;
        bladeColliders[1].enabled = false;
        DashEffect.SetActive(false);
        isAttacking = false;
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    void ChasePlayer()
    {

        if (!statsScript)
            statsScript = target.GetComponent<Stats>(); // tell player he is fighting
        if (!statsScript.fighting)
            statsScript.fighting = true;
        BossAnimation.SetBool("IsWalking", true);
        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Move the boss towards the target
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

        // Rotate the boss to face the target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));

        // Keep the Boss upright by resetting the X and Z rotation
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    IEnumerator SwipeAttack()
    {
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Rotate the boss to face the player instantly
        rb.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        bladeColliders[0].enabled = true;
        bladeColliders[1].enabled = true;
        BossAnimation.SetBool("IsWalking", false);
        BossAnimation.SetBool("AttackSlash", true);
        WhattypeOfAttack = "Slash";
        isAttacking = true;
        yield return new WaitForSeconds(0.4f);
        bladeColliders[0].enabled = false;
        bladeColliders[1].enabled = false;
        yield return new WaitForSeconds(1.4f);
        isAttacking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerObj")
        {
            Stats stats = target.GetComponent<Stats>();
           if(WhattypeOfAttack == "Slash")
           {
                stats.TakeSlashDamage(slashDamage);
           }
           else if(WhattypeOfAttack == "Pierce")
            {
                stats.TakepierceDamage(pierceDamage);
            }
        }
    }
}
