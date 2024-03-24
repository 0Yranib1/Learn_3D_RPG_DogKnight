using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;

    public GameObject rockPrefab;

    public Transform handPos;
    
    //Animation Event
    public void KickOff()
    {
        if(attackTarget!=null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStatus>();

            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            targetStats.TakeDamage(characterStatus, targetStats);
        }
    }
    
    //Animation Event
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }

    protected override void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange() )
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }else if (TargetInSkillRange())
        {
            //技能攻击
            anim.SetTrigger("Skill");
        }
    }
}
