using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rock : MonoBehaviour
{
    private Rigidbody rb;
    [Header("Basic Settings")] 
    public float force;

    public GameObject target;
    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
}
