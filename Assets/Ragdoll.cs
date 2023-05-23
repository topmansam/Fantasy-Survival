using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Animator animator;
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    public void DeactivateRagdoll() {
        foreach (var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        animator.enabled = true;
    }
    public void ActivateRagdoll()
    {
        foreach(var rigidBody in rigidBodies)
        {
            // rigidBody.isKinematic = false;
            animator.SetTrigger("death");
            agent.enabled = false;
            
            Destroy(gameObject, 2f);
        }
        //animator.enabled = false; 
    }

       
}
