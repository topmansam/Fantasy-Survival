using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        // public TrailRenderer tracer; // Commented out the TrailRenderer
        public int bounce;
    }

    public LayerMask playerLayerMask;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public int maxBounces = 0;
    public bool debug = false;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    // public TrailRenderer tracerEffect; // Commented out the TrailRenderer
    public string weaponName;
    private AmmoWidget ammoWidget;
    public static int totalAmmo_ref;
    public static int clipSize_ref;
    public int ammoCount;
    public int clipSize;
    public int totalAmmo = 24;
    public float damage = 10f;
    public Transform raycastOrigin;
    public Transform raycastDestination;
    public WeaponRecoil recoil;
    public GameObject magazine;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifetime = 3.0f;

    private void Awake()
    {
        totalAmmo_ref = totalAmmo;
        clipSize_ref = clipSize;

        ammoCount=clipSize;
        recoil = GetComponent<WeaponRecoil>();
        ammoWidget= FindObjectOfType<AmmoWidget>();
        ammoWidget.Refresh(ammoCount,totalAmmo);
    }

    Vector3 GetPosition(Bullet bullet)
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        // bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity); // Commented out the TrailRenderer instantiation
        // bullet.tracer.AddPosition(position); // Commented out the TrailRenderer addition of the position
        bullet.bounce = maxBounces;

        return bullet;
    }

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0.0f;
        recoil.Reset();
    }

    public void UpdateWeapon(float deltaTime)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            StartFiring();
        }
        if (isFiring)
        {
            UpdateFiring(deltaTime);
        }
        UpdateBullets(deltaTime);
        if (Input.GetButtonUp("Fire1"))
        {
            StopFiring();
        }
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet => {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifetime);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        Color debugColor = Color.green;
        int layerMask = ~playerLayerMask;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            Debug.Log("Object Hit: " + hitInfo.collider.gameObject.name);

            // Check the layer of the hit object
            int hitObjectLayer = hitInfo.collider.gameObject.layer;
            Debug.Log("Hit Object Layer: " + LayerMask.LayerToName(hitObjectLayer));

            // Check the player layer mask value
            int playerLayerMaskValue = playerLayerMask.value;
            Debug.Log("Player Layer Mask Value: " + playerLayerMaskValue);

            // Verify if the player layer mask is correctly set
            bool ignorePlayerLayer = (playerLayerMaskValue & (1 << hitObjectLayer)) != 0;
            Debug.Log("Ignore Player Layer: " + ignorePlayerLayer);
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.time = maxLifetime;
            end = hitInfo.point;
            debugColor = Color.red;



            //// Collision impulse
            //var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            //if (rb2d)
            //{
            //    rb2d.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
            //}
            var hitbox = hitInfo.collider.GetComponent<Hitbox>();
            if (hitbox)
            {
                hitbox.onRaycastHit(this, ray.direction);
            }

            // bullet.tracer.transform.position = end; // Commented out the TrailRenderer position update

            if (debug)
            {
                Debug.DrawLine(start, end, debugColor, 1.0f);
            }
        }
    }

    private void FireBullet()
    {
        if (ammoCount <= 0)
        {
            return;
        }
        ammoCount--;

        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1);
        }

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);

        recoil.GenerateRecoil(weaponName);
        ammoWidget.Refresh(ammoCount, totalAmmo);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
    public void Restore()
    {
        totalAmmo = totalAmmo_ref;
        clipSize = clipSize_ref;
        ammoCount = clipSize;
    }
}