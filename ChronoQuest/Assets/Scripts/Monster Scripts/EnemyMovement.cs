using System.Collections;
using UnityEngine;

public class WizardMovement : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float moveSpeed = 2f;
    public int patrolDestination = 0;
    public float patrolWaitTime = 1f;

    [Header("Chase Settings")]
    public Transform playerTransform;
    public bool isChasing = false;
    public float chaseDistance = 5f;
    public float stopChaseDistance = 3f;
    public float returnDelay = 1.5f;

    // Internal state
    private float leftLimit;
    private float rightLimit;
    private bool isReturningToPatrol = false;
    private bool isWaitingAtPatrolPoint = false;

    public Animator anim;

    public bool isStunned = false;
    public float stunTime = 0.9f;
    public float stunCounter = 0f;


    [Header("Attack Settings")]
    public EnemyAttack attackScript;
    public float attackDistance = 1f; // distance to player to trigger attack

    private void Start()
    {
        if (patrolPoints.Length < 2)
        {
            Debug.LogError("Please assign at least 2 patrol points!");
            enabled = false;
            return;
        }

        leftLimit = patrolPoints[0].position.x;
        rightLimit = patrolPoints[1].position.x;
        transform.localScale = (patrolDestination == 0) ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);

        attackScript = GetComponent<EnemyAttack>();
        //anim = GetComponentInChildren<Animator>();

        SetMoving(false); // start idle
    }

    private void Update()
    {
        // If the enemy is stunned (hit), stop movement
        if (isStunned)
        {
            stunCounter -= Time.deltaTime;
            if (stunCounter <= 0)
                isStunned = false;

            SetMoving(false);
            return;
        }

        if (anim != null)
        {
            Debug.Log("Animator isMoving: " + anim.GetBool("isMoving"));
        }

        if (isReturningToPatrol || isWaitingAtPatrolPoint)
        {
            SetMoving(false);
            return;
        }

        if (isChasing)
        {
            DoChase();
        }
        else
        {
            DoPatrol();
        }
    }


    private void DoPatrol()
    {
        // Start chase if player gets close
        if (Vector2.Distance(transform.position, playerTransform.position) < chaseDistance)
        {
            isChasing = true;
            return;
        }

        SetMoving(true);

        // Move towards patrol points
        Transform targetPoint = patrolPoints[patrolDestination];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            Vector3 newScale = (patrolDestination == 0) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            int nextDestination = (patrolDestination == 0) ? 1 : 0;
            StartCoroutine(WaitAtPatrolPoint(nextDestination, newScale));
        }
    }

    private void DoChase()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (playerTransform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        // Stop chasing if player is too far
        if (distanceToPlayer > stopChaseDistance)
        {
            StartCoroutine(ReturnToPatrol());
            return;
        }

        // If player is within attack range
        if (distanceToPlayer <= attackDistance)
        {
            // Stop moving while attacking
            SetMoving(false);

            // Only attack if not currently attacking (handled by EnemyAttack cooldown)
            if (attackScript != null && attackScript.CanAttack())
            {
                attackScript.TryAttack();
            }

            return; // do not move while in attack range
        }

        // Only chase if within patrol bounds
        if (transform.position.x < leftLimit || transform.position.x > rightLimit)
        {
            StartCoroutine(ReturnToPatrol());
            return;
        }

        // Move toward player
        SetMoving(true);
        Vector3 direction = (playerTransform.position.x < transform.position.x) ? Vector3.left : Vector3.right;
        transform.localScale = (direction == Vector3.left) ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }


    private IEnumerator WaitAtPatrolPoint(int nextDestination, Vector3 newScale)
    {
        isWaitingAtPatrolPoint = true;
        SetMoving(false);

        yield return new WaitForSeconds(patrolWaitTime);

        patrolDestination = nextDestination;
        transform.localScale = newScale;

        isWaitingAtPatrolPoint = false;
    }

    private IEnumerator ReturnToPatrol()
    {
        if (isReturningToPatrol)
            yield break;

        isReturningToPatrol = true;
        isChasing = false;

        SetMoving(false);

        yield return new WaitForSeconds(returnDelay);

        float distToPoint0 = Vector2.Distance(transform.position, patrolPoints[0].position);
        float distToPoint1 = Vector2.Distance(transform.position, patrolPoints[1].position);

        if (distToPoint0 < distToPoint1)
        {
            patrolDestination = 0;
            transform.localScale = (transform.position.x > patrolPoints[0].position.x) ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        }
        else
        {
            patrolDestination = 1;
            transform.localScale = (transform.position.x < patrolPoints[1].position.x) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
        }

        isReturningToPatrol = false;
    }

    // ---------------------
    // ANIMATION HELPERS
    // ---------------------
    private void SetMoving(bool moving)
    {
        if (anim != null)
        {
            anim.SetBool("isMoving", moving);
            Debug.Log("Set isMoving = " + moving);
        }
    }

}
