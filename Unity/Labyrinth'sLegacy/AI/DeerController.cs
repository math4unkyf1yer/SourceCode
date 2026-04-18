using UnityEngine;

public class DeerController : EnemyBaseController
{
    public int escapeRange = 7;
    public int safeDistance = 55;
    public float escapeSpeed = 10f;
    private bool escaping;

    protected override float GroundCheckLength => 1.9f;

    public Animator deerAnimator;

    protected override void Start()
    {
        base.Start();
        SetupPatrolPoints();
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        if (!isGrounded) return;

        Vector3 rayOrigin = eyes.transform.position;
        Vector3 rayDirection = eyes.transform.forward;
        RaycastHit hit;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        if (distanceToPlayer <= escapeRange && !playerMovementScript.crouching || escaping)
        {
            Escape();
            if (distanceToPlayer > safeDistance)
                escaping = false;
        }
        else if (Physics.Raycast(rayOrigin, rayDirection, out hit, sightDistance, targetLayer))
        {
            Escape();
        }
        else if (!escaping)
        {
            Patrol();
        }
    }

    void Escape()
    {
        deerAnimator.SetBool("isRunning", true);
        deerAnimator.SetBool("isWalking", false);
        escaping = true;

        Vector3 escapeDirection = -(target.position - transform.position).normalized;
        rb.MovePosition(transform.position + escapeDirection * escapeSpeed * Time.fixedDeltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(escapeDirection);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * speed));
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    protected override void Patrol()
    {
        deerAnimator.SetBool("isWalking", true);
        deerAnimator.SetBool("isRunning", false);
        base.Patrol();
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleOuterMapCollision(collision);
    }
}
