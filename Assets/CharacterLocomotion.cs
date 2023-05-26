using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    public Animator rigController;
    public float jumpHeight;
    public float gravity;
    public float stepDown;
    public float airControl;
    public float jumpDamp;
    public float groundSpeed;
    public float pushPower;
    public bool isSprinting;
    Animator animator;
    CharacterController cc;
    ActiveWeapon activeWeapon;
    ReloadWeapon reloadWeapon;
    CharacterAiming characterAiming;
    Vector2 input;

    Vector3 rootMotion;
    Vector3 velocity;
    bool isJumping;

    int isSprintingParam = Animator.StringToHash("isSprinting");

    [HideInInspector] public StaminaController staminaController;

    // Start is called before the first frame update
    void Start()
    {
        staminaController = GetComponent<StaminaController>();
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        activeWeapon = GetComponent<ActiveWeapon>();
        reloadWeapon = GetComponent<ReloadWeapon>();
        characterAiming = GetComponent<CharacterAiming>();
    }

    public void SetRunSpeed(bool speed)
    {
        //groundSpeed = speed;
        if (IsSprinting()) speed = true;
        else speed = false;
       
    }
    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        animator.SetFloat("InputX", input.x);
        animator.SetFloat("InputY", input.y);

        UpdateIsSprinting();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (!IsSprinting())
        {
            staminaController.weAreSprinting = false;
        }
        if (IsSprinting())
        {
            if (staminaController.playerStamina > 0.1)
            {
                staminaController.weAreSprinting = true;
                staminaController.Sprinting();
            }
            else
            {
                staminaController.weAreSprinting = false;
            }
        }
    }

    bool IsSprinting()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isFiring = activeWeapon.IsFiring();
        bool isReloading = reloadWeapon.isReloading;
        bool isChangingWeapon = activeWeapon.isChangingWeapon;
        bool isAiming = characterAiming.isAiming;
        return isSprinting && !isFiring && !isReloading && !isChangingWeapon && !isAiming;
    }

    private void UpdateIsSprinting()
    {
        if (staminaController.playerStamina > 0.01&& !staminaController.waitTillFullRegen)
        {
            groundSpeed = 1.2f;
            isSprinting = IsSprinting();
            animator.SetBool(isSprintingParam, isSprinting);
            rigController.SetBool(isSprintingParam, isSprinting);
        }
        else
        {
            groundSpeed = 0.5f;
            isSprinting = false;
            animator.SetBool(isSprintingParam, false);
            rigController.SetBool(isSprintingParam, false);
        }
    }


    private void OnAnimatorMove()
    {
        rootMotion += animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        if (isJumping)
        { // IsInAir state
            UpdateInAir();
        }
        else
        { // IsGrounded state
            UpdateOnGround();
        }
    }

    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        cc.Move(stepForwardAmount + stepDownAmount);
        rootMotion = Vector3.zero;

        if (!cc.isGrounded)
        {
            SetInAir(0);
        }
    }

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        cc.Move(displacement);
        isJumping = !cc.isGrounded;
        rootMotion = Vector3.zero;
        animator.SetBool("isJumping", isJumping);
    }

    Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    void Jump()
    {
        if (!isJumping)
        {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
        animator.SetBool("isJumping", true);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }
}
