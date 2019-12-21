using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float rangeRadius;
    public float chaseRadius;
    [Range(0, 360)] public float viewAngle;

    [HideInInspector] public LayerMask playerMask;
    [HideInInspector] public LayerMask obstacleMask;
    public List<Transform> visibleTargets = new List<Transform>();

    public Transform player;

    EnemyStateMachine stateMachine;

    void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
        obstacleMask = LayerMask.GetMask("Obstacle");

        stateMachine = GetComponent<EnemyStateMachine>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * rangeRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * rangeRadius);

        Gizmos.color = Color.red;
        foreach (Transform visibleTarget in visibleTargets)
        {
            Gizmos.DrawLine(transform.position, visibleTarget.position);
        }
    }

    public void FindPlayer()
    {
        visibleTargets.Clear();
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, rangeRadius, playerMask);

        if (targetInRange.Length == 0)
        {
            return;
        }
        else
        {
            Transform target = targetInRange[0].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float disToTarget = Vector3.Distance(transform.position, target.position);

                if (!(Physics.Raycast(transform.position, dirToTarget, disToTarget, obstacleMask)))
                {
                    stateMachine.switchState(EnemyStateMachine.StateType.Shoot);
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0.0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
