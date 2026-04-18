using System.Collections;
using UnityEngine;

public class BossController : EnemyBaseController
{
    public float chaseRange = 20f;
    public float attackRange = 3f;
    public float dashSpeed = 2f;
    public float dashCooldown;
    private bool dashReady = true;
    private int slashDamage = 8;
    private int pierceDamage = 15;
    public BoxCollider[] bladeColliders;
    public bool isAttacking = false;
    private string WhattypeOfAttack;

    public Animator BossAnimation;
    public GameObject DashEffect;

    private AudioSource bossSound;
    public AudioClip dashSound;

    protected override void Start()
    {
        base.Start();
        bossSound = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);

            if (distanceToPlayer <= attackRange && !isAttacking)
                StartCoroutine(SwipeAttack());

            if (distanceToPlayer >= 8 && distanceToPlayer < 13 && !isAttacking && dashReady)
                StartCoroutine(AttackDash());

            if (distanceToPlayer <= chaseRange && !isAttacking)
                ChasePlayer();
            else
            {
                if (statsScript != null)
                {
                    statsScript.fighting = false;
                    statsScript = null;
                }
            }
        }
    }

    protected override void ChasePlayer()
    {
        BossAnimation.SetBool("IsWalking", true);
        base.ChasePlayer();
    }

    IEnumerator AttackDash()
    {
        bossSound.clip = dashSound;
        bossSound.Play();
        DashEffect.SetActive(true);
        dashReady = false;
        WhattypeOfAttack = "Pierce";
        isAttacking = true;
        BossAnimation.SetBool("IsWalking", false);
        BossAnimation.SetTrigger("AttackDash");
        bladeColliders[0].enabled = true;
        bladeColliders[1].enabled = true;

        yield return new WaitForSeconds(0.6f);

        Vector3 targetPosition = target.position;
        Vector3 dashDirection = (targetPosition - transform.position).normalized;
        float dashDuration = Vector3.Distance(transform.position, targetPosition) / dashSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.fixedDeltaTime));
            elapsedTime += Time.fixedDeltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition);

        yield return new WaitForSeconds(0.5f);
        bladeColliders[0].enabled = false;
        bladeColliders[1].enabled = false;
        DashEffect.SetActive(false);
        isAttacking = false;
        yield return new WaitForSeconds(dashCooldown);
        dashReady = true;
    }

    IEnumerator SwipeAttack()
    {
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        rb.rotation = Quaternion.Euler(0, Quaternion.LookRotation(directionToPlayer).eulerAngles.y, 0);
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
        if (other.gameObject.name == "PlayerObj")
        {
            Stats stats = target.GetComponent<Stats>();
            if (WhattypeOfAttack == "Slash")
                stats.TakeSlashDamage(slashDamage);
            else if (WhattypeOfAttack == "Pierce")
                stats.TakepierceDamage(pierceDamage);
        }
    }
}
