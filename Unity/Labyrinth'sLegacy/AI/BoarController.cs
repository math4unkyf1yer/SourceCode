using System.Collections;
using UnityEngine;

public class BoarController : EnemyBaseController
{
    public float chaseRange = 15f;
    public float attackRange = 9f;
    public float dashSpeed = 25f;
    public bool boarStop;
    public bool isAttacking;
    public GameObject dashEffect;

    [Header("Animation")]
    public Animator boarAnimation;

    private bool chasingPlayer;
    private bool waitForChase;
    private bool waitForReset;
    private string attackType;
    private int slashDamage = 6;
    private int pierceDamage = 10;

    protected override void Start()
    {
        base.Start();
        SetupPatrolPoints();
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        if (!isGrounded) return;

        if (target == null || boarStop) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        RaycastHit hit;

        if (distanceToPlayer <= attackRange && !isAttacking)
            StartCoroutine(AttackDash());

        if (distanceToPlayer <= chaseRange && !isAttacking && !waitForChase && !playerMovementScript.crouching ||
            distanceToPlayer <= 20 && chasingPlayer && !isAttacking && !waitForChase)
        {
            ChasePlayer();
            chasingPlayer = true;
        }
        else if (Physics.Raycast(eyes.transform.position, eyes.transform.forward, out hit, sightDistance, targetLayer) && !isAttacking)
        {
            ChasePlayer();
        }
        else if (!isAttacking && !waitForChase)
        {
            Patrol();
            chasingPlayer = false;
        }
    }

    protected override void ChasePlayer()
    {
        if (!isGrounded) return;
        SetWalkAnimation();
        base.ChasePlayer();
    }

    protected override void Patrol()
    {
        if (!isGrounded || startPositions.Length == 0) return;

        if (statsScript != null)
        {
            statsScript.fighting = false;
            statsScript = null;
        }

        SetWalkAnimation();

        Transform patrolPoint = startPositions[currentPatrolIndex];
        Vector3 targetPos = new Vector3(patrolPoint.position.x, patrolPoint.position.y, patrolPoint.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        Vector3 horizontalVelocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        rb.velocity = horizontalVelocity;

        if (horizontalVelocity.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
        }

        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(targetPos.x, 0, targetPos.z)) < patrolThreshold)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % startPositions.Length;
        }
    }

    private void SetWalkAnimation()
    {
        if (!boarAnimation.GetBool("Walking"))
        {
            boarAnimation.SetBool("Walking", true);
            boarAnimation.SetBool("Running", false);
        }
    }

    private void SetDashAnimation()
    {
        boarAnimation.SetBool("Walking", false);
        boarAnimation.SetBool("Running", true);
    }

    IEnumerator AttackDash()
    {
        if (waitForReset) yield break;

        dashEffect.SetActive(true);
        attackType = "Pierce";
        waitForChase = true;
        waitForReset = true;
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);
        SetDashAnimation();

        Vector3 targetPosition = target.position;
        Vector3 dashDirection = (targetPosition - transform.position).normalized;
        float overshootDistance = 5f;
        Vector3 dashTargetPosition = targetPosition + dashDirection * overshootDistance;
        float leapDuration = (8f + overshootDistance) / dashSpeed;

        float leapTimer = 0f;
        while (leapTimer < leapDuration)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.fixedDeltaTime));
            rb.rotation = Quaternion.LookRotation(dashDirection);
            leapTimer += Time.fixedDeltaTime;
            yield return null;
        }

        while (Vector3.Distance(transform.position, dashTargetPosition) > 0.5f)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, dashTargetPosition, dashSpeed * Time.fixedDeltaTime));
            yield return null;
        }

        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
        isAttacking = false;
        yield return new WaitForSeconds(1f);
        waitForChase = false;
        dashEffect.SetActive(false);
        yield return new WaitForSeconds(2f);
        waitForReset = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player" && isAttacking)
        {
            Stats stats = collision.gameObject.GetComponent<Stats>();
            if (attackType == "Pierce")
                stats.TakepierceDamage(pierceDamage);
            isAttacking = false;
        }
        HandleOuterMapCollision(collision);
    }
}
