﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DemonMovementController : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 3f;
    public float optimalYDistance = 5f;
    public float optimalYDistanceThreshold = 0.5f;
    public float stopDistance = 5f;

    private PlayerModel playerModel;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GameManager.Instance.playerModel;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerModel.camouflageModeController.isCamouflaged)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 moveDelta = Vector3.zero;

        var dir = playerModel.playerTarget.position - transform.position;
        var targetPos = playerModel.playerTarget.position;

        var xzDir = dir;
        xzDir.y = 0;
        targetPos -= xzDir.normalized * stopDistance;
        targetPos.y += optimalYDistance;

        dir = targetPos - transform.position;
        if (dir.magnitude > stopDistance)
        {
            moveDelta = dir.normalized * speed * Time.deltaTime;
        }

        /*float optimalYPos = playerModel.playerTarget.position.y + optimalYDistance;
        float yDist = optimalYPos - transform.position.y;
        if (Mathf.Abs(yDist) > optimalYDistanceThreshold)
        {
            moveDelta.y = Mathf.Sign(yDist) * speed * Time.deltaTime;
        }*/

        transform.position += moveDelta;

        Quaternion targetRot =
            Quaternion.LookRotation((playerModel.playerTarget.position - transform.position).normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}