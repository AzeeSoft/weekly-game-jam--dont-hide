using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public PlayerInputController playerInputController { get; private set; }
    public PlayerMovementController playerMovementController { get; private set; }
    public CamouflageModeController camouflageModeController { get; private set; }
    public Animator animator { get; private set; }
    public Health health { get; private set; }
    public bool isCamouflaged => camouflageModeController.isCamouflaged;
    public bool isAlive => health.currentHealth > 0;
    public bool isDead => !isAlive;

    public float delayBeforeHealthRegeneration = 3f;
    public float healthRegenerationSpeed = 1f;
    public Transform playerTarget;
    public float deathAnimationDuration = 3f;
    public float playerHitCamShakeDuration = 1f;
    public float playerHitCamShakeMinInterval = 1f;
    public float playerHitCamShakeAmplitude = 3f;
    public int playerHitCamShakeFrequency = 10;

    private bool isShakingCam = false;

    void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerMovementController = GetComponent<PlayerMovementController>();
        camouflageModeController = GetComponent<CamouflageModeController>();
        animator = GetComponentInChildren<Animator>(false);
        health = GetComponent<Health>();
        health.OnDamageTaken.AddListener(() =>
        {
            if (health.currentHealth > 0)
            {
                animator.SetTrigger("Hit");

                if (!isShakingCam)
                {
                    CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.CamNoise(
                        playerHitCamShakeAmplitude, playerHitCamShakeFrequency);
                    isShakingCam = true;
                    this.WaitAndExecute(() =>
                    {
                        CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.CamNoise(0, 0);
                        this.WaitAndExecute(() =>
                        {
                            isShakingCam = false;
                        }, playerHitCamShakeMinInterval);
                    }, playerHitCamShakeDuration);
                }
            }
        });
        health.OnHealthDepleted.AddListener(() =>
        {
            animator.applyRootMotion = true;
            animator.SetTrigger("Death");
        });
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