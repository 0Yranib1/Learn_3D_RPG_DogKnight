using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates{GUARD,PATROL,CHASE,DEAD}//简易状态枚举 警戒 巡逻 追击 死亡
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    
    [Header("Basic Settings")] 
    public float sightRadius;//可视范围
    public bool isGuard;
    private float speed;
    private GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;

    [Header("Patrol State")] 
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    //配合动画
    private bool isWalk;
    private bool isChase;
    private bool isFollow;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }
    
    private void Start()
    {
        
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    // Update is called once per frame
    void Update()
    {
        SwitchStates();
        SwithcAnimation();
    }

    void SwithcAnimation()
    {
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow",isFollow);
    }
    
    //怪物状态切换
    void SwitchStates()
    {
        //发现玩家切换到追击状态
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        
        switch (enemyStates)
        {
            case EnemyStates.GUARD :
                break;
            case EnemyStates.PATROL:
                //不在追击状态
                isChase = false;
                agent.speed = speed * 0.5f;
                //判断是否走到巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                
                break;
            case EnemyStates.CHASE:
                //追击玩家
                isWalk = false;
                isChase = true;
                speed = agent.speed;
                if (!FoundPlayer())
                {
                    //超出范围回到上一个状态
                    isFollow = false;
                    agent.destination = transform.position;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }
                //在攻击范围内攻击
                //配合动画
                break;
            case EnemyStates.DEAD:
                break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y,
            guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? randomPoint : transform.position;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }
}
