using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    private bool isWalking = false;
    public GameObject player;
    public Animator enemyAnimator;
    public float damage = 20f;
    public float health = 100;
    [SerializeField]
    public GameManager gameManager;
    public Slider slider;

    public bool playerInReach;
    private float attackDelayTimer;
    public float attackAnimStartDelay;
    public float delayBetweenAttacks;

    //public AudioSource audioSource;
    //public AudioClip[] zombieSounds;

    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player");
        slider.maxValue = health;
        slider.value = health;
    }

    void Update()
    {
        /*
        if (!audioSource.isPlaying)
        {
            audioSource.clip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.Play();
        }

        */
        slider.transform.LookAt(player.transform);

        GetComponent<NavMeshAgent>().destination = player.transform.position;

        if (GetComponent<NavMeshAgent>().velocity.magnitude > 1 && !isWalking && !enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            enemyAnimator.SetBool("run", true);
            enemyAnimator.SetBool("walk", false);
            isWalking = true;
        }
        else if (GetComponent<NavMeshAgent>().velocity.magnitude <= 1 && isWalking)
        {
            enemyAnimator.SetBool("walk", true);
            enemyAnimator.SetBool("run", false);
            isWalking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInReach = true;
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        slider.value = health;
        if (health <= 0)
        {
            enemyAnimator.SetTrigger("death");
            gameManager.enemiesAlive--;
            Destroy(gameObject, 10f);
            Destroy(GetComponent<NavMeshAgent>());
            Destroy(GetComponent<EnemyManager>());
            Destroy(GetComponent<CapsuleCollider>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (playerInReach)
        {
            attackDelayTimer += Time.deltaTime;
        }
        else
        {
            playerInReach = false;
        }

        if (attackDelayTimer >= delayBetweenAttacks - attackAnimStartDelay && attackDelayTimer <= delayBetweenAttacks && playerInReach)
        {
            enemyAnimator.SetTrigger("attack1");
        }

        if (attackDelayTimer >= delayBetweenAttacks && playerInReach)
        {
            player.GetComponent<PlayerManager>().Hit(damage);
            attackDelayTimer = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("leaving:");
            playerInReach = false;
            attackDelayTimer = 0;
        }
    }
}
