using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    //配合动画
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
}
