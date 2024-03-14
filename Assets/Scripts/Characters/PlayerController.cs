using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void MoveToTarget(Vector3 target)
    {
        agent.destination = target;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchAnimation();
    }

    void SwitchAnimation()
    {
        anim.SetFloat("Speed",agent.velocity.sqrMagnitude);
    }
}
