using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
   private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float BulletSpeed = 100;
    public float damage = 25f;
    public GameObject hitParticles;
    public AudioClip gunShot;
    public AudioSource audioSource;
    // private Animator Animator;
    private float LastShootTime;
     

    private void Awake()
    {
        //Animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot(Vector3 targetPosition)
    {
         
        Debug.Log("Shoot method called with target position: " + targetPosition);
        if (LastShootTime + ShootDelay < Time.time)
        {
            if (gameObject.activeInHierarchy && audioSource.enabled) // Add this check
            {
                audioSource.PlayOneShot(gunShot);
            }
         
            ShootingSystem.Play();
            Vector3 direction = (targetPosition - BulletSpawnPoint.position).normalized;

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                Debug.Log("Raycast hit: " + hit.transform.name);
                // Check if the hit object has the EnemyManager component
                EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
                if (enemyManager != null)
                {
                    // Apply damage to the enemy
                    enemyManager.Hit(damage);
                    Debug.Log("Raycast hit: " + hit.transform.name);
                    // Rest of the code
                    Debug.Log("Damage applied: " + damage + " to enemy: " + hit.transform.name);

                    // Create hitParticles at the hit point
                    GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                    instParticles.transform.parent = hit.transform;
                    Destroy(instParticles, 2f);
                }
                if (gameObject.activeInHierarchy) // Check if the game object is active
                {
                    TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));
                }
               
                LastShootTime = Time.time;
            }
            else
            {
                Debug.Log("Raycast did not hit any colliders with the specified layer mask.");
                // Rest of the code
                if (gameObject.activeInHierarchy) // Check if the game object is active
                {
                    TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                    StartCoroutine(SpawnTrail(trail, targetPosition, Vector3.zero, false));
                }
            
                LastShootTime = Time.time;
            }
        }
    }


    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
          
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }
        //.SetBool("IsShooting", false);
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
