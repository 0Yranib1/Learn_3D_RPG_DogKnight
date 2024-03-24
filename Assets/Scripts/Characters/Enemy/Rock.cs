using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    public enum RockStates
    {
        HitPlayer,HitEnemy,HitNothing
    }
    private Rigidbody rb;
    [Header("Basic Settings")] 
    public float force;
    public int damage;
    public RockStates rockStates;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        flyToTarget();
    }

    public void flyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = target.transform.position - transform.position+Vector3.up;
        direction.Normalize();
        rb.AddForce(direction*force,ForceMode.Impulse);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStatus>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStatus>());
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStatus>();
                    otherStats.TakeDamage(damage,otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }

    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }
}
