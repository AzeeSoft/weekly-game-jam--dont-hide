﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public PlayerInputController playerInputController { get; private set; }
    public CamouflageModeController camouflageModeController { get; private set; }
    public Animator animator { get; private set; }
    public Health health { get; private set; }
    public bool isCamouflaged => camouflageModeController.isCamouflaged;
    public bool isAlive => health.currentHealth > 0;
    public bool isDead => !isAlive;

    public float delayBeforeHealthRegeneration = 3f;
    public float healthRegenerationSpeed = 1f;
    public Transform playerTarget;

    void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        camouflageModeController = GetComponent<CamouflageModeController>();
        animator = GetComponentInChildren<Animator>(false);
        health = GetComponent<Health>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive && health.currentHealth < health.maxHealth &&
            camouflageModeController.timeSinceLastCamouflage > delayBeforeHealthRegeneration &&
            health.timeSinceLastDamage > delayBeforeHealthRegeneration)
        {
            health.UpdateHealth(healthRegenerationSpeed * Time.deltaTime);
        }
    }
}