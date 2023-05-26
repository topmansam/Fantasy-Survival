using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StaminaController : MonoBehaviour
{
    public float playerStamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [SerializeField] private float jumpCost = 20f;
    [HideInInspector] public bool hasRengerated = true;
    [HideInInspector] public bool weAreSprinting = false;

    [Range(0, 50)][SerializeField] private float staminaDrain = 0.5f;
    [Range(0, 50)][SerializeField] private float staminaRegen = 0.5f;

    [SerializeField] private float slowedRunSpeed = 1.2f;
    [SerializeField] private float normalRunSPeed = 2f;

    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    private bool isDelayingRegen = false;
    private float regenDelayTimer = 0.02f;
    private float delayDuration = 3f;
    private bool isShiftKeyDown = false;
    public bool waitTillFullRegen = false;
    private CharacterLocomotion locomotion;

    private void Start()
    {
        locomotion = GetComponent<CharacterLocomotion>();

    }

    private void Update()
    {
        if (!weAreSprinting)
        {
            
             if (playerStamina <= maxStamina - 0.01)
            {
                playerStamina += staminaRegen * Time.deltaTime;
                UpdateStamina(1);
                if (playerStamina >= maxStamina)
                {
                    locomotion.SetRunSpeed(1.2f);
                    sliderCanvasGroup.alpha = 0;
                    hasRengerated = true;
                }
            }
            
        }
    }
    public void Sprinting()
    {

        if (hasRengerated)
        {
            weAreSprinting = true;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            if (playerStamina <= 0)
            {
                Debug.Log("Empty");
                hasRengerated = false;
              
                locomotion.SetRunSpeed(slowedRunSpeed);

                sliderCanvasGroup.alpha = 0;
            }
        }
    }
    public void StaminaJump()
    {
        if (playerStamina >= (maxStamina * jumpCost / maxStamina))
        {
            playerStamina -= jumpCost;
            UpdateStamina(1);
        }
    }

    void UpdateStamina(int value)
    {
        staminaProgressUI.fillAmount = playerStamina / maxStamina;

        if (value == 0)
        {
            sliderCanvasGroup.alpha = 0;
        }
        else
        {
            sliderCanvasGroup.alpha = 1;
        }
    }
}
