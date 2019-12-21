﻿using System.Collections;
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
    private ObjectPooler.Key enemyProjectileKey = ObjectPooler.Key.EnemyProjectile;

    [HideInInspector] public bool searchingStop = false;
    [HideInInspector] public bool isConfused = false;
     public float fireTime = 0.0f;

    [HideInInspector] public float searchTime;
    public float searchBuffer;

    [HideInInspector] public float shootTime;
    public float shootBuffer;

    public Transform[] navPoints;
    public float maxBulletSpread;
    public float timeToMaxSpread;


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

        Shoot();
    }

    void Shoot()
    {
        if (Time.time - shootTime >= shootBuffer)
        {
            GameObject pooledObj = ObjectPooler.GetPooler(enemyProjectileKey).GetPooledObject();

            Vector3 targetDirection = fov.player.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            float currentSpread = Mathf.Lerp(0.0f, maxBulletSpread, fireTime / timeToMaxSpread);

            targetRotation = Quaternion.RotateTowards(targetRotation, Random.rotation, Random.Range(0, currentSpread));


            pooledObj.transform.position = transform.position;
            pooledObj.transform.rotation = targetRotation;

            pooledObj.SetActive(true);

            shootTime = Time.time;
            fireTime += Time.deltaTime * 5;
        }
        
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