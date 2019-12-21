using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    FieldOfView fov;
    EnemyStateMachine stateMachine;
    NavMeshAgent nav;

    Vector3 lastPlayerPosition;

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

    public void Shooting()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.rangeRadius, fov.playerMask);

        if (targetInRange.Length == 0)
        {
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
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);

            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        Debug.Log("Shooting player");
    }

    public void Chasing()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.chaseRadius, fov.playerMask);
        Transform target = targetInRange[0].transform;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float disToTarget = Vector3.Distance(transform.position, target.position);

        if (targetInRange.Length == 0 || Physics.Raycast(transform.position, dirToTarget, disToTarget, fov.obstacleMask))
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
        if (Vector3.Distance(transform.position, lastPlayerPosition) < .5f && !searchingStop)
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
                stateMachine.switchState(EnemyStateMachine.StateType.Patrol);
            }
        }
    }
}