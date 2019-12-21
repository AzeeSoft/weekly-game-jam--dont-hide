using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    FieldOfView fov;
    EnemyStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent nav;

    Vector3 lastPlayerPosition;
    int currentPoint = 0;

    [HideInInspector] public bool searchingStop = false;
    [HideInInspector] public bool isConfused = false;

    [HideInInspector] public float searchTime;
    public float searchBuffer;

    public Transform[] navPoints;

    void Awake()
    {
        fov = GetComponent<FieldOfView>();
        stateMachine = GetComponent<EnemyStateMachine>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        nav.SetDestination(navPoints[currentPoint].position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(lastPlayerPosition, new Vector3(lastPlayerPosition.x, lastPlayerPosition.y + 0.5f, lastPlayerPosition.z));
    }

    public void Shooting()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.rangeRadius, fov.playerMask);

        if (targetInRange.Length == 0)
        {
            nav.isStopped = false;
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
            

            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
            return;
        }

        Transform target = targetInRange[0].transform;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float disToTarget = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, dirToTarget, disToTarget, fov.obstacleMask))
        {
            nav.isStopped = false;
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);

            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(fov.player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);

        Debug.Log("Shooting player");
    }

    public void Chasing()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.chaseRadius, fov.playerMask);

        if (targetInRange.Length == 0)
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        Transform target = targetInRange[0].transform;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float disToTarget = Vector3.Distance(transform.position, target.position);

        if (Physics.Raycast(transform.position, dirToTarget, disToTarget, fov.obstacleMask))
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);

            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        if (Vector3.Distance(lastPlayerPosition, fov.player.position) > fov.rangeRadius)
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
        }
    }

    public void playerSearch()
    {
        if (Vector3.Distance(transform.position, lastPlayerPosition) < 1.5f && !searchingStop)
        {
            searchingStop = true;
            searchTime = Time.time;
            return;
        }

        if (searchingStop)
        {
            if (!isConfused)
            {
                Debug.Log(gameObject.name + " is confused!");
                isConfused = true;
            }
            
            if (Time.time - searchTime >= searchBuffer)
            {
                Debug.Log(gameObject.name + " is now patrolling!");

                nav.SetDestination(navPoints[currentPoint].position);
                stateMachine.switchState(EnemyStateMachine.StateType.Patrol);
                return;
            }
        }
    }

    public void Patrolling()
    {
        if (Vector3.Distance(transform.position, navPoints[currentPoint].position) < 1.5f)
        {
            if (currentPoint + 1 == navPoints.Length)
            {
                currentPoint = 0;
            }
            else
            {
                currentPoint++;
            }

            nav.SetDestination(navPoints[currentPoint].position);
        }
    }
}