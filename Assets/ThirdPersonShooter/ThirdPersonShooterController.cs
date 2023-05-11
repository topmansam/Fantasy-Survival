using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{

    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    public Gun gun;
    public Gun pistolGun;
    public Gun assaultRifleGun;
    public GameObject pistol;
    public GameObject assaultRifle;
    private int pistolLayerIndex;
    private int assaultRifleLayerIndex;

    private float LastShootTime;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    public float aimRigWeight;
    



    private void Awake()
    {

        gun = pistolGun;
        pistol.SetActive(true);
        assaultRifle.SetActive(false);

        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        pistolLayerIndex = animator.GetLayerIndex("PistolLayer");
        assaultRifleLayerIndex = animator.GetLayerIndex("AssaultRifleLayer");

       

    }
    

    private void Update()
    {
        SwitchWeapon();
        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        float defaultDistance = 10f; // Step 1: Add a default distance value

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        else
        {
            mouseWorldPosition = ray.GetPoint(defaultDistance); // Step 2: Update mouseWorldPosition based on whether the raycast hit something
            debugTransform.position = mouseWorldPosition;
        }

        if (starterAssetsInputs.aim)
        {
            aimRigWeight = 1f;
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            if (pistol.activeSelf)
            {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));

                animator.SetLayerWeight(assaultRifleLayerIndex, 0f);
            }
            else if (assaultRifle.activeSelf)
            {
                animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 13f));
                animator.SetLayerWeight(pistolLayerIndex, 0f);
            }

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimRigWeight = 0f;
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);

            animator.SetLayerWeight(pistolLayerIndex, 0f);
            animator.SetLayerWeight(assaultRifleLayerIndex, 0f);
        }

        
    }
    private void SwitchWeapon()
    {
        if (starterAssetsInputs.switchToPistol)
        {
            pistol.SetActive(true);
            assaultRifle.SetActive(false);
            gun = pistolGun; // Set the gun variable to the pistol's Gun script instance
            starterAssetsInputs.switchToPistol = false;
           // Debug.Log("Switched to Pistol");
        }
        else if (starterAssetsInputs.switchToAssaultRifle)
        {
            pistol.SetActive(false);
            assaultRifle.SetActive(true);
            gun = assaultRifleGun; // Set the gun variable to the assault rifle's Gun script instance
            starterAssetsInputs.switchToAssaultRifle = false;
            //Debug.Log("Switched to Assault Rifle");
        }
    }

}
