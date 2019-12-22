using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAttackController : MonoBehaviour
{
    public float attackRadius = 3f;
    public float damageRate = 20f;

    private DemonModel demonModel;
    private PlayerModel playerModel => demonModel.playerModel;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    // Start is called before the first frame update
    void Start()
    {
        demonModel = GetComponent<DemonModel>();
    }

    // Update is called once per frame
    void Update()
    {
        if (demonModel.canSeePlayer)
        {
            float distToPlayer = Vector3.Distance(transform.position, playerModel.playerTarget.position);
            if (distToPlayer < attackRadius)
            {
                AttackPlayer();
            }
        }
    }

    private void AttackPlayer()
    {
        playerModel.health.TakeDamage(damageRate * Time.deltaTime);
    }
}
