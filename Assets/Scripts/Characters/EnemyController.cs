using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyStates{GUARD,PATROL,CHASE,DEAD}//简易状态枚举 警戒 巡逻 追击 死亡
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStatus))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{

    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    protected CharacterStatus characterStatus;
    [Header("Basic Settings")] 
    public float sightRadius;//可视范围
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;
    private float remainLookAtTime;
    private Collider coll;

    [Header("Patrol State")] 
    public float patrolRange;
    private Vector3 wayPoint;
    private Quaternion guardRotation;
    private Vector3 guardPos;
    //配合动画
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDead;
    private bool playerDead;
    private float lastAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        anim = GetComponent<Animator>();
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        characterStatus = GetComponent<CharacterStatus>();
        coll = GetComponent<Collider>();
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
        //FIXME:场景切换后修改
        GameManager.Instance.AddObserver(this);
    }
    //切换场景时启用
    // private void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    private void OnDisable()
    {
        if(!GameManager.IsInitialized)
            return;
        
        GameManager.Instance.RemoveObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterStatus.CurrentHealth ==0)
        {
            isDead = true;
        }
        if(!playerDead)
        {
            SwitchStates();
            SwithcAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwithcAnimation()
    {
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase",isChase);
        anim.SetBool("Follow",isFollow);
        anim.SetBool("Critical",characterStatus.isCritical);
        anim.SetBool("Death",isDead);
    }
    
    //怪物状态切换
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //发现玩家切换到追击状态
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
        
        switch (enemyStates)
        {
            case EnemyStates.GUARD :
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                }

                if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                }
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
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //在攻击范围内攻击
                if (TargetInSkillRange() || TargetInAttackRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime <= 0)
                    {
                        lastAttackTime = characterStatus.attackData.coolDown;
                        //暴击判断
                        characterStatus.isCritical = Random.value < characterStatus.attackData.criticalChance;
                        //攻击
                        Attack();
                    }
                }
                //配合动画
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                // agent.enabled = false;
                Destroy(gameObject,2f);
                break;
        }
    }

    void Attack()
    {

        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange() && !TargetInSkillRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击
            anim.SetTrigger("Skill");
        }
    }
    
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStatus.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <=
                   characterStatus.attackData.skillRange;
        }
        else
        {
            return false;
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
    
    //Animation Event
    //攻击动画调用伤害事件
    void Hit()
    {
        if(attackTarget!=null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStatus>();
            targetStats.TakeDamage(characterStatus, targetStats);
        }
        
    }

    public void EndNotify()
    {
        //獲勝動畫
        anim.SetBool("Win",true);
        playerDead = true;
        //停止所有移動
        //停止Agent
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
