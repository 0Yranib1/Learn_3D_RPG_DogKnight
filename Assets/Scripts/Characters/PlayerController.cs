using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStatus characterStatus;
    private GameObject attackTarget;

    private float lastAttackTime;
    private bool isDead;
    private float stopDistance;
    private float originalAttackRange;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStatus = GetComponent<CharacterStatus>();
        stopDistance = agent.stoppingDistance;
        originalAttackRange = characterStatus.attackData.attackRange;
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if(isDead) return;


        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if(isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStatus.isCritical = Random.value < characterStatus.attackData.criticalChance;
            if (attackTarget.CompareTag("AttackAble"))
            {
                characterStatus.attackData.attackRange = originalAttackRange + 0.5f;
            }else if (attackTarget.CompareTag("Enemy"))
            {
                characterStatus.attackData.attackRange = originalAttackRange;
            }
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //携程
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStatus.attackData.attackRange;
        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position,transform.position)>characterStatus.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        //攻击 
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical",characterStatus.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间 攻击动作
            lastAttackTime = characterStatus.attackData.coolDown;
        }
    }
    
    
    private void OnEnable()
    {

    }
    
    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStatus);
    }
    
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized)
        {
            return;
        }
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = characterStatus.CurrentHealth == 0;
        
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    void SwitchAnimation()
    {
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);
        anim.SetBool("Death",isDead);
    }
    
    //Animation Event
    //攻击动画调用伤害事件
    void Hit()
    {
        if (attackTarget.CompareTag("AttackAble"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward*20,ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStatus>();
        
            targetStats.TakeDamage(characterStatus,targetStats);
        }
    }
}
