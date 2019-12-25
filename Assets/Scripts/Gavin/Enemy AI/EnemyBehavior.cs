using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    FieldOfView fov;
    EnemyStateMachine stateMachine;

    [HideInInspector] public NavMeshAgent nav;

    public EnemyAnimationCheck animCheck;
    public Transform bulletHole;
    public Animator anim;
    public TMPro.TextMeshProUGUI textMesh;
    public Animator textAnim;
    public Canvas confusionCanvas;

    public AudioClip[] confusedSounds;
    public AudioClip shootingSound;
    

    Vector3 lastPlayerPosition;
    int currentPoint = 0;
    private ObjectPooler.Key enemyProjectileKey = ObjectPooler.Key.EnemyProjectile;

    Quaternion originalRotation;

    [HideInInspector] public bool searchingStop = false;
    [HideInInspector] public bool isConfused = false;
    [HideInInspector] public bool isShocked = false;
     public float fireTime = 0.0f;

    [HideInInspector] public float searchTime;
    public float searchBuffer;

    [HideInInspector] public float shootTime;
    public float shootBuffer;

    public Transform[] navPoints;
    public float maxBulletSpread;
    public float timeToMaxSpread;


    void Awake()
    {
        fov = GetComponent<FieldOfView>();
        stateMachine = GetComponent<EnemyStateMachine>();
        nav = GetComponent<NavMeshAgent>();

        originalRotation = transform.rotation;
    }

    void Start()
    {
        nav.SetDestination(navPoints[currentPoint].position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(lastPlayerPosition, new Vector3(lastPlayerPosition.x, lastPlayerPosition.y + 0.5f, lastPlayerPosition.z));
    }

    void Update()
    {
        CanvasBillBoard();
    }

    void CanvasBillBoard()
    {
        Vector3 dirToTarget = (fov.player.position - transform.position).normalized;
        dirToTarget.y = 0;
        confusionCanvas.transform.forward = dirToTarget;
    }

    public void Shooting()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.rangeRadius, fov.playerMask);

        if (!isShocked)
        {
            textAnim.SetBool("Shocked", true);
            anim.SetTrigger("Shocked");
            textMesh.color = Color.red;
            textMesh.text = "!";
            isShocked = true;

        }


        if (targetInRange.Length == 0)
        {
            nav.isStopped = false;
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
            

            stateMachine.switchState(EnemyStateMachine.StateType.Chase);
            return;
        }

        Transform target = fov.player;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float disToTarget = Vector3.Distance(transform.position, target.position);

        RaycastHit outRay;

        if (Physics.Raycast(transform.position, dirToTarget, out outRay, disToTarget, fov.obstacleMask, QueryTriggerInteraction.Ignore))
        {
            nav.isStopped = false;
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);

            Debug.Log(outRay.collider.gameObject.name);

            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(GameManager.Instance.playerModel.playerTarget.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);

        Shoot();
    }

    void Shoot()
    {
        if (Time.time - shootTime >= shootBuffer)
        {
            SoundEffectsManager.Instance.PlayAt(shootingSound, transform.position);

            GameObject pooledObj = ObjectPooler.GetPooler(enemyProjectileKey).GetPooledObject();

            Vector3 targetDirection = GameManager.Instance.playerModel.playerTarget.transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            float currentSpread = Mathf.Lerp(0.0f, maxBulletSpread, fireTime / timeToMaxSpread);

            targetRotation = Quaternion.RotateTowards(targetRotation, Random.rotation, Random.Range(0, currentSpread));


            pooledObj.transform.position = bulletHole.position;
            pooledObj.transform.rotation = targetRotation;
            pooledObj.transform.rotation = targetRotation;
            pooledObj.transform.rotation = targetRotation;

            pooledObj.SetActive(true);

            shootTime = Time.time;
            fireTime += Time.deltaTime * 5;
        }
        
    }

    public void CamoCheck()
    {
        if (GameManager.Instance.playerModel.isCamouflaged)
        {
            Debug.Log("Player disappeared?");

            textAnim.SetBool("Shocked", false);
            textAnim.SetBool("Confusion", false);

            nav.isStopped = true;
            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
        }
    }

    public void Chasing()
    {
        Collider[] targetInRange = Physics.OverlapSphere(transform.position, fov.chaseRadius, fov.playerMask);

        if (Vector3.Distance(lastPlayerPosition, fov.player.position) > 2f)
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
        }

        if (targetInRange.Length == 0)
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);
            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }

        Transform target = fov.player;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float disToTarget = Vector3.Distance(transform.position, target.position);

        RaycastHit outRay;

        if (Physics.Raycast(transform.position, dirToTarget, out outRay, disToTarget, fov.obstacleMask, QueryTriggerInteraction.Ignore))
        {
            lastPlayerPosition = fov.player.position;
            nav.SetDestination(lastPlayerPosition);

            Debug.Log(outRay.collider.gameObject.name);

            stateMachine.switchState(EnemyStateMachine.StateType.LostPlayer);
            return;
        }
    }

    public void playerSearch()
    {
        if (Vector3.Distance(transform.position, lastPlayerPosition) < 1.5f && !searchingStop)
        {
            searchingStop = true;
            searchTime = Time.time;
            nav.isStopped = true;
            return;
        }
        else if (GameManager.Instance.playerModel.isCamouflaged && !searchingStop)
        {
            searchingStop = true;
            searchTime = Time.time;
            nav.isStopped = true;
            return;
        }

        if (searchingStop)
        {
            if (!isConfused)
            {
                textMesh.color = new Color { r = 207, g = 203, b = 0, a = 1 };
                textMesh.text = "?";

                textAnim.SetBool("Confusion", true);
                anim.SetTrigger("Confused");

                int rand = Random.Range(0, 2);
                switch (rand)
                {
                    case 0:
                        SoundEffectsManager.Instance.PlayAt(confusedSounds[0], transform.position);
                        break;
                    case 1:
                        SoundEffectsManager.Instance.PlayAt(confusedSounds[1], transform.position);
                        break;
                }

                isConfused = true;
            }
            
            if (animCheck.stopConfusion)
            {
                Debug.Log(gameObject.name + " is now patrolling!");

                nav.SetDestination(navPoints[currentPoint].position);
                stateMachine.switchState(EnemyStateMachine.StateType.Patrol);
                return;
            }
        }
    }

    public void Patrolling()
    {
        if (Vector3.Distance(transform.position, navPoints[currentPoint].position) < 1.5f)
        {
            if (navPoints.Length > 1)
            {
                if (currentPoint + 1 == navPoints.Length)
                {
                    currentPoint = 0;
                }
                else
                {
                    currentPoint++;
                }

                nav.SetDestination(navPoints[currentPoint].position);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, 5f * Time.deltaTime);
                nav.isStopped = true;
                anim.SetBool("Idle", true);
            }
        }
    }
}