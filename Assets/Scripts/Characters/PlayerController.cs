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
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStatus = GetComponent<CharacterStatus>();
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;
            characterStatus.isCritical = Random.value < characterStatus.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //携程
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
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
    
    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = characterStatus.CurrentHealth == 0;
        
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
        var targetStats = attackTarget.GetComponent<CharacterStatus>();
        
        targetStats.TakeDamage(characterStatus,targetStats);
    }
}
