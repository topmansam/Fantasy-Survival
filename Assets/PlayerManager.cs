using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public PlayerHealthBar healthBar;
    public float maxHealth = 100;
    public float currentHealth;
    public float healthRegenerationDelay = 5f; // Time in seconds before health starts regenerating
    public float healthRegenerationRate = 10f; // Health regeneration rate per second
    public TextMeshProUGUI healthNum;
    public GameManager gameManager;
    public GameObject playerCamera;
    public CanvasGroup hurtPanel;
    private float shakeTime;
    private float shakeDuration;
    private Quaternion playerCameraOriginalRotation;
    private bool isRegeneratingHealth = false;
    private float timeSinceLastHit;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRegeneratingHealth)
        {
            timeSinceLastHit += Time.deltaTime;
            if (timeSinceLastHit >= healthRegenerationDelay)
            {
                Debug.Log("regen");
                RegenerateHealth();
            }
        }

        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }

        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if (playerCamera.transform.localRotation != playerCameraOriginalRotation)
        {
            playerCamera.transform.localRotation = playerCameraOriginalRotation;
        }

        //if (Input.GetKeyDown(KeyCode.N)){
        //    Hit(20);
        //}
    }

    public void Hit(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        healthNum.text = maxHealth.ToString() + " Health ";

        if (currentHealth <= 0)
        {
            //gameManager.EndGame();
            Debug.Log("GAME ENDED");
        }
        else
        {
            isRegeneratingHealth = true;
            timeSinceLastHit = 0f;
            //shakeTime = 0;
            //shakeDuration = .2f;
            //hurtPanel.alpha = .7f;
        }
    }

    private void RegenerateHealth()
    {
        currentHealth += healthRegenerationRate * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        healthBar.SetHealth(currentHealth);
        healthNum.text = maxHealth.ToString() + " Health ";

        if (currentHealth >= maxHealth)
        {
            isRegeneratingHealth = false;
        }
    }

    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }
}