﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public CamouflageModeController camouflageModeController { get; private set; }
    public Health health { get; private set; }
    public bool isCamouflaged => camouflageModeController.isCamouflaged;

    public float delayBeforeHealthRegeneration = 3f;
    public float healthRegenerationSpeed = 1f;
    public Transform playerTarget;

    void Awake()
    {
        camouflageModeController = GetComponent<CamouflageModeController>();
        health = GetComponent<Health>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (health.currentHealth < health.maxHealth &&
            camouflageModeController.timeSinceLastCamouflage > delayBeforeHealthRegeneration)
        {
            health.UpdateHealth(healthRegenerationSpeed * Time.deltaTime);
        }
    }
}