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
                enemy.Patrolling();
                fov.FindPlayer();
                break;
            case StateType.Shoot:
                enemy.Shooting();
                break;
            case StateType.Chase:
                enemy.Chasing();
                fov.FindPlayer();
                break;
            case StateType.LostPlayer:
                enemy.playerSearch();
                fov.FindPlayer();
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
                enemy.isConfused = false;
                break;
            case StateType.Shoot:
                enemy.searchingStop = false;
                enemy.isConfused = false;
                enemy.nav.isStopped = true;
                break;
            case StateType.Chase:
                enemy.fireTime = 0.0f;
                break;
            case StateType.LostPlayer:
                enemy.fireTime = 0.0f;
                break;
        }
    }
}
