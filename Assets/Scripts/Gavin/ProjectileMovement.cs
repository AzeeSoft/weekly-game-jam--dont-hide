using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    Rigidbody rb;
    float timer;
    public float bulletSpeed;
    public float damageValue;
    [Range(0, 1)] public float autoProbability;

    bool isAuto = false;

    private Vector3 prevPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        timer = Time.time;
        rb.velocity = transform.forward * bulletSpeed;

        float rand = Random.Range(0, 1);
        if (rand < autoProbability)
        {
            isAuto = true;
        }

        prevPosition = transform.position;
    }

    void OnObjectHit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (other.gameObject.tag == "Player")
        {
            if (!GameManager.Instance.playerModel.isCamouflaged)
            {
                GameManager.Instance.playerModel.health.TakeDamage(damageValue);
            }

            //Debug.Log("Player hit by " + gameObject.name);
        }

        if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Projectile")
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isAuto && !GameManager.Instance.playerModel.isCamouflaged)
        {
            Vector3 dirToTarget = (GameManager.Instance.playerModel.playerTarget.position - transform.position)
                .normalized;
            rb.velocity = dirToTarget * bulletSpeed;
        }

        if (Time.time - timer >= 5f)
        {
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        var layerMask = HelperUtilities.GetOpaqueLayerMaskForRaycast();
        var hits = Physics.RaycastAll(prevPosition, transform.position - prevPosition,
            Vector3.Distance(prevPosition, transform.position),
            layerMask, QueryTriggerInteraction.Ignore);

        foreach (var hit in hits)
        {
            OnObjectHit(hit.collider);
        }

        prevPosition = transform.position;
    }

    void OnDisable()
    {
        rb.velocity = Vector3.zero;
        isAuto = false;
    }
}