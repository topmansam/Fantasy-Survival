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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find the player GameObject using the tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player GameObject with tag 'Player' not found.");
        }
    }

    private void Update()
    {
        if (health.alive && playerTransform != null)
        {
            agent.destination = playerTransform.position;
            //timer -= Time.deltaTime;

            //if (timer < 0.0f)
            //{
            //    float sqrDistance = (playerTransform.position - agent.destination).sqrMagnitude;

            //    if (sqrDistance > maxDistance * maxDistance)
            //    {
            //        
            //    }

            //    timer = maxTime;
            //}

            animator.SetFloat("speed", agent.velocity.magnitude);
        }
    }
}
