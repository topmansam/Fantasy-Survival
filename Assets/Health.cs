using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public Animator animator;
    public GameManager gameManager;
    UIHealthBar healthBar;
   public bool alive = true;
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    Ragdoll ragdoll;
    SkinnedMeshRenderer skinnedMeshRenderer;
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        ragdoll = GetComponent<Ragdoll>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        currentHealth = maxHealth;
        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        healthBar = GetComponentInChildren<UIHealthBar>(); 
        foreach(var rigidBody in rigidBodies)
        {
            Hitbox hitbox = rigidBody.gameObject.AddComponent<Hitbox>();
            hitbox.health = this;
        }
      
    }

    private void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1.0f;
        skinnedMeshRenderer.material.color = Color.white *intensity;
    }
    public void TakeDamage(float amount, Vector3 direction)
    {
        currentHealth -= amount;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
        if (currentHealth <= 0.0f)
        {
             
            Die();
        }
        blinkTimer = blinkDuration;
    }
    private void Die() {
        alive = false;
        //ragdoll.ActivateRagdoll();
        animator.SetTrigger("death");
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(GetComponent<EnemyManager>());
        Destroy(GetComponent<CapsuleCollider>());
        healthBar.gameObject.SetActive(false);
        gameManager.enemiesAlive--;
        Destroy(gameObject, 2f);
         
         

    }
}
