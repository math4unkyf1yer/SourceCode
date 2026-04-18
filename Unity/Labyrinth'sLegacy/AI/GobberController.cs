using System.Collections;
using UnityEngine;

public class GobberController : EnemyBaseController
{
    public float chaseRange;
    public float attackRange;
    public GameObject poop;
    public Transform poopLocation;

    private float chaseSpeed = 7f;
    private float poopRange = 13f;
    private bool isStopped;
    private bool chasingPlayer;
    private bool canAttack = true;
    private bool isAttacking;
    private bool isThrowingPoop;
    private bool hasReachedThrowPosition;
    private bool hasHitPlayer;
    private string attackType;
    private int blundDamage = 13;
    private HitBox gobberHealth;
    private Animator gobberAnimation;

    protected override float ChaseSpeed => chaseSpeed;

    protected override void Start()
    {
        base.Start();
        gobberHealth = GetComponent<HitBox>();
        gobberAnimation = GetComponent<Animator>();
        SetupPatrolPoints();
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        if (!isGrounded) return;

        if (target == null || isStopped) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (gobberHealth.health <= 3)
        {
            if (!isThrowingPoop)
                ThrowPoop();
            return;
        }

        if (distanceToPlayer <= attackRange && !isAttacking && canAttack)
            Attack();

        if (distanceToPlayer <= chaseRange && !isAttacking && !playerMovementScript.crouching ||
            distanceToPlayer <= 20 && chasingPlayer && !isAttacking)
        {
            ChasePlayer();
            chasingPlayer = true;
        }
        else if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out RaycastHit hit, sightDistance, targetLayer) && !isAttacking)
        {
            ChasePlayer();
        }
        else if (!isAttacking)
        {
            Patrol();
            chasingPlayer = false;
        }
    }

    protected override void ChasePlayer()
    {
        gobberAnimation.SetBool("IsRunningGober", true);
        gobberAnimation.SetBool("IsWalkingGobber", false);
        base.ChasePlayer();
    }

    protected override void Patrol()
    {
        gobberAnimation.SetBool("IsWalkingGobber", true);
        gobberAnimation.SetBool("IsRunningGober", false);
        base.Patrol();
    }

    private void ThrowPoop()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer < poopRange && !hasReachedThrowPosition)
        {
            Vector3 directionAway = (transform.position - target.position).normalized;
            gobberAnimation.SetBool("IsRunningGober", true);
            gobberAnimation.SetBool("IsWalkingGobber", false);

            rb.MovePosition(transform.position + directionAway * speed * Time.fixedDeltaTime);

            Quaternion lookRotation = Quaternion.LookRotation(directionAway);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
            rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
        }
        else
        {
            hasReachedThrowPosition = true;
            isThrowingPoop = true;
            gobberAnimation.SetBool("IsRunningGober", false);
            StartCoroutine(FaceAndThrowPoop());
        }
    }

    IEnumerator FaceAndThrowPoop()
    {
        Quaternion lookAtPlayer = Quaternion.LookRotation((target.position - transform.position).normalized);
        while (Quaternion.Angle(rb.rotation, lookAtPlayer) > 1f)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, lookAtPlayer, Time.fixedDeltaTime * speed * 2);
            yield return new WaitForFixedUpdate();
        }

        gobberAnimation.SetTrigger("ThrowPoop");
        GameObject poopObj = Instantiate(poop, poopLocation.position, Quaternion.identity);
        poopObj.GetComponent<PoopScript>().SetTargetPosition(target.position);

        yield return new WaitForSeconds(2f);
        isThrowingPoop = false;
    }

    private void Attack()
    {
        canAttack = false;
        gobberAnimation.SetBool("IsRunningGober", false);
        gobberAnimation.SetTrigger("IsAttacking");
        isAttacking = true;
        attackType = "blund";
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        hasHitPlayer = false;
        isAttacking = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj" && isAttacking && !hasHitPlayer)
        {
            hasHitPlayer = true;
            Stats stats = target.gameObject.GetComponent<Stats>();
            if (attackType == "blund")
                stats.TakebleugDamage(blundDamage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleOuterMapCollision(collision);
    }
}
