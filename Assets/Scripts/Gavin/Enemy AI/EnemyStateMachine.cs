using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    FieldOfView fov;
    EnemyBehavior enemy;

    void Awake()
    {
        fov = GetComponent<FieldOfView>();
        enemy = GetComponent<EnemyBehavior>();
    }

    void Start()
    {
        OnStateEnter();
    }

    public enum StateType
    {
        Patrol,
        Shoot,
        Chase,
        LostPlayer
    }

    public StateType state = StateType.Patrol;

    void Update()
    {
        switch (state)
        {
            case StateType.Patrol:
                if (!GameManager.Instance.playerModel.isCamouflaged)
                {
                    fov.FindPlayer();
                }

                enemy.Patrolling();
                break;
            case StateType.Shoot:
                if (!GameManager.Instance.playerModel.isCamouflaged)
                {
                    enemy.Shooting();
                }

                enemy.CamoCheck();
                break;
            case StateType.Chase:
                if (!GameManager.Instance.playerModel.isCamouflaged)
                {
                    enemy.Chasing();
                    fov.FindPlayer();
                }
                
                enemy.CamoCheck();
                break;
            case StateType.LostPlayer:
                if (!GameManager.Instance.playerModel.isCamouflaged)
                {
                    fov.FindPlayer();
                }

                enemy.playerSearch();
                break;
        }
    }

    public void switchState(StateType newState)
    {
        state = newState;
        OnStateEnter();
    }

    public void OnStateEnter()
    {
        switch (state)
        {
            case StateType.Patrol:
                enemy.searchingStop = false;
                enemy.animCheck.stopConfusion = false;
                enemy.isConfused = false;
                enemy.nav.isStopped = false;

                enemy.anim.SetBool("Walking", true);
                enemy.anim.ResetTrigger("Confused");

                enemy.textAnim.SetBool("Confusion", false);
                break;
            case StateType.Shoot:
                enemy.searchingStop = false;
                enemy.animCheck.stopConfusion = false;
                enemy.isConfused = false;

                enemy.anim.SetBool("Walking", false);
                enemy.anim.SetBool("Chasing", false);
                enemy.anim.ResetTrigger("Confused");

                enemy.textAnim.SetBool("Confusion", false);

                enemy.nav.isStopped = true;
                break;
            case StateType.Chase:
                enemy.anim.SetBool("Chasing", true);
                enemy.anim.SetBool("Walking", false);

                enemy.fireTime = 0.0f;
                break;
            case StateType.LostPlayer:
                enemy.fireTime = 0.0f;

                enemy.anim.SetBool("Walking", true);
                enemy.anim.SetBool("Chasing", false);
                enemy.anim.ResetTrigger("Shocked");

                enemy.isShocked = false;
                enemy.textAnim.SetBool("Shocked", false);
                break;
        }
    }
}
