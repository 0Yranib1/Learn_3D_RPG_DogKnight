using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates{GUARD,PATROL,CHASE,DEAD}//简易状态枚举 警戒 巡逻 追击 死亡
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{

    public EnemyStates enemyStates;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchStates();
    }
    
    //怪物状态切换
    void SwitchStates()
    {
        switch (enemyStates)
        {
            case EnemyStates.GUARD :
                break;
            case EnemyStates.PATROL:
                break;
            case EnemyStates.CHASE:
                break;
            case EnemyStates.DEAD:
                break;
        }
    }
}
