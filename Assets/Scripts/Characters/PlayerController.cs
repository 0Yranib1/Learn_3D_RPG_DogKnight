using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
        
    }
}
