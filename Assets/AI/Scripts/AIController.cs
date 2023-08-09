using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private SphereCollider checkRange;

    private int destPoint;
    private bool seenPlayer, isMoving, isPunching, isDead;
    private float fieldOfView = 110f;
    private float angle;
    public bool IsPunching { get => isPunching; }

    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] wayPoints;

    public int Health { get; set; } = 4;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        checkRange = GetComponent<SphereCollider>();

        GoToNextPoint();        
    }

    void Update()
    {
        if(Health == 0)
        {
            isDead = true;
        }

        MoveAI();

        UpdateAnimation();
    }

    void GoToNextPoint()
    {
        destPoint = Random.Range(0, wayPoints.Length);
        agent.SetDestination(wayPoints[destPoint].position);
    }

    void MoveAI()
    {
        if (isDead)
        {
            agent.isStopped = true;
            return;
        }
        else
        {
            if (seenPlayer)
            {
                agent.stoppingDistance = 2.5f;
                agent.speed = 5f;
                agent.SetDestination(player.transform.position);
            }
            else
            {
                agent.speed = 1.5f;
                agent.stoppingDistance = 1f;

                if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
                {
                    StartCoroutine(Wait(2f));

                    if (!isMoving)
                        GoToNextPoint();
                }
                else
                {
                    isMoving = true;
                }
            }
        }

        if(seenPlayer)
        {
            agent.SetDestination(player.transform.position);
            agent.speed = 5f;

            if(!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
            {
                if (!isPunching)
                    StartCoroutine(Punch());
                else
                    agent.speed = 5f;
            }

            agent.isStopped = isPunching;
        }
    }

    void UpdateAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);
        anim.SetBool("IsAttacking", seenPlayer);
        anim.SetBool("IsDead", isDead);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            Vector3 direction = other.transform.position - transform.position;
            angle = Vector3.Angle(direction, transform.forward);

            if(angle < fieldOfView * 0.5f)
            {
                if (Physics.Linecast(transform.position + transform.up * 1.6f, player.transform.position, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == player)
                        seenPlayer = true;
                    else
                        seenPlayer = false;
                }
                else
                {
                    seenPlayer = false;
                }
            }
            else
            {
                seenPlayer = false;
            }
        }

        print(seenPlayer);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            seenPlayer = false;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position + transform.up * 1.6f, player.transform.position, Color.red);
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        isMoving = false;
    }

    IEnumerator Punch()
    {
        yield return new WaitForSeconds(1f);
        isPunching = true;
        yield return new WaitForSeconds(2.2f);
        isPunching = false;
    }
}
