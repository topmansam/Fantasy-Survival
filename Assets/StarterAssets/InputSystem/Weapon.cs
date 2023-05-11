using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Camera cam;
    private RaycastHit rayHit;
    private StarterAssetsExtra controls;
    [SerializeField] private float bulletRange;
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magazineSize;
    private bool isShooting, readyToShoot, reloading;
    private int ammoLeft;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private float bulletHoleLifeSpan;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField] string EnemyTag;
    public float damage = 25f;
    public GameObject hitParticles;
    private float BulletSpeed = 100;

    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private TrailRenderer BulletTrail;
    private void Awake()
    {
        controls = new StarterAssetsExtra();
        ammoLeft = magazineSize;
        readyToShoot = true;
        cam = Camera.main;
        controls.Player.Shoot.started += ctx => StartShot();
        controls.Player.Shoot.canceled += ctx => EndShot();
        controls.Player.Reload.performed += ctx => Reload();
    }
    private void Update()
    {
        if(isShooting && readyToShoot && !reloading && ammoLeft > 0)
        {
            PerformShot(); 
        }
    }
    private void StartShot() {
        isShooting = true;

    }
    private void EndShot() {
        isShooting = false;
    }

    private void PerformShot()
    {
        //Debug.Log("Im Shooting");
        readyToShoot = false;
        Vector3 direction = cam.transform.forward;

        if (Physics.Raycast(cam.transform.position, direction, out rayHit, bulletRange))
        {
            // Instantiate the bullet trail regardless of whether the raycast hit an enemy or not
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);

            Debug.Log(rayHit.collider.gameObject.name);
            if (rayHit.collider.gameObject.tag == EnemyTag)
            {
                EnemyManager enemyManager = rayHit.transform.GetComponent<EnemyManager>();
                if (enemyManager != null)
                {
                    enemyManager.Hit(damage);
                    GameObject instParticles = Instantiate(hitParticles, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                    instParticles.transform.parent = rayHit.transform;
                    Destroy(instParticles, 2f);
                }
                StartCoroutine(SpawnTrail(trail, rayHit.point, rayHit.normal, true));
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, rayHit.point, rayHit.normal, false));
                GameObject bulletHole = Instantiate(bulletHolePrefab, rayHit.point + rayHit.normal * 0.001f, Quaternion.identity) as GameObject;
                bulletHole.transform.LookAt(rayHit.point + rayHit.normal);
                Destroy(bulletHole, bulletHoleLifeSpan);
            }
        }
        else
        {
            // If the raycast did not hit anything, spawn the trail to the end of the bulletRange
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
            Vector3 endPoint = BulletSpawnPoint.position + direction * bulletRange;
            StartCoroutine(SpawnTrail(trail, endPoint, Vector3.zero, false));
        }

        muzzleFlash.Play();
        ammoLeft--;

        if (ammoLeft >= 0)
        {
            Invoke("ResetShot", fireRate);
            if (!isAutomatic)
            {
                EndShot();
            }
        }
    }



    private void ResetShot() {
        readyToShoot = true;
        

    }
    private void Reload() {
        reloading = true;
        //Debug.Log("Im Reloading");
        Invoke("ReloadFinish", reloadTime);
    }
    private void ReloadFinish() {
        ammoLeft = magazineSize;
        reloading = false;
    }
    private void OnEnable()
    {
        controls.Enable();
     
    }

    private void OnDisable()
    {
        controls.Disable();
       
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
