using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AiLocomotion : MonoBehaviour
{
    public Health health;
    public Transform playerTransform;
    NavMeshAgent agent;   
     Animator animator;
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.alive)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                float sqrDistance = (playerTransform.position - agent.destination).sqrMagnitude;
                if (sqrDistance > maxDistance * maxDistance)
                {
                    agent.destination = playerTransform.position;
                }
                timer = maxTime;
            }

            animator.SetFloat("speed", agent.velocity.magnitude);
        }
    }
}
