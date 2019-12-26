using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DemonMovementController : MonoBehaviour
{
    public float chaseSpeed = 4f;
    public float returnSpeed = 2f;
    public float rotationSpeed = 3f;
    public float optimalYDistance = 5f;
    public float optimalYDistanceThreshold = 0.5f;
    public float stopDistance = 5f;

    private DemonModel demonModel;
    private PlayerModel playerModel => demonModel.playerModel;
    private Vector3 originalPos;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

    // Start is called before the first frame update
    void Start()
    {
        demonModel = GetComponent<DemonModel>();
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (demonModel.canSeePlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            MoveTowardsTargetPosition(originalPos, returnSpeed);
        }
    }

    private void MoveTowardsPlayer()
    {
        var dir = playerModel.playerTarget.position - transform.position;
        var targetPos = playerModel.playerTarget.position;

        var xzDir = dir;
        xzDir.y = 0;
        targetPos -= xzDir.normalized * stopDistance;
        targetPos.y += optimalYDistance;

        MoveTowardsTargetPosition(targetPos, chaseSpeed);
    }

    private void MoveTowardsTargetPosition(Vector3 targetPos, float speed)
    {
        Vector3 moveDelta = Vector3.zero;

        var dir = targetPos - transform.position;
        if (dir.magnitude > stopDistance)
        {
            moveDelta = dir.normalized * speed * Time.deltaTime;
        }

        transform.position += moveDelta;

        Quaternion targetRot =
            Quaternion.LookRotation((playerModel.playerTarget.position - transform.position).normalized);
        demonModel.avatar.transform.rotation = Quaternion.Lerp(demonModel.avatar.transform.rotation, targetRot,
            rotationSpeed * Time.deltaTime);

        var rotAngles = demonModel.minimapIcon.transform.rotation.eulerAngles;
        rotAngles.y = demonModel.avatar.transform.rotation.eulerAngles.y;
        demonModel.minimapIcon.transform.rotation = Quaternion.Euler(rotAngles);
    }
}