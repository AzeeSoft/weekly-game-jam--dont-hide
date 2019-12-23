﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    Rigidbody rb;
    float timer;
    public float bulletSpeed;
    public float damageValue;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        timer = Time.time;
        rb.velocity = transform.forward * bulletSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.playerModel.health.TakeDamage(damageValue);
            //Debug.Log("Player hit by " + gameObject.name);
        }

        if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "Projectile")
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.time - timer >= 5f)
        {
            gameObject.SetActive(false);
        }
    }

    void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
