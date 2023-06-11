using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        // public TrailRenderer tracer; // Commented out the TrailRenderer
        public int bounce;
    }
    public float falloffRange;
    public float modifiedDamage;
    public LayerMask playerLayerMask;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public bool isFiring = false;
    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;

    public int maxBounces = 0;
    public bool debug = false;
    public ParticleSystem[] muzzleFlash;
    public ParticleSystem hitEffect;
    // public TrailRenderer tracerEffect; // Commented out the TrailRenderer
    public string weaponName;
    public AmmoWidget ammoWidget;
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
    public float accumulatedTime;
    public List<Bullet> bullets = new List<Bullet>();
    float maxLifetime = 3.0f;

    private void Awake()
    {
        totalAmmo_ref = totalAmmo;
        clipSize_ref = clipSize;

        ammoCount = clipSize;
        recoil = GetComponent<WeaponRecoil>();
        ammoWidget = FindObjectOfType<AmmoWidget>();
        ammoWidget.Refresh(ammoCount, totalAmmo);
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down;
        Vector3 displacement = bullet.initialVelocity * bullet.time;
        Vector3 drop = 0.5f * gravity * bullet.time * bullet.time;
        return bullet.initialPosition + displacement + drop;
    }

    public Bullet CreateBullet(Vector3 position, Vector3 velocity)
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
    public virtual void UpdateFiring(float deltaTime)
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

    //Raycast segment without clamp
    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        Ray ray = new Ray(start, direction);

        Color debugColor = Color.green;
        int layerMask = ~playerLayerMask;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask))
        {
            Debug.Log("Object Hit: " + hitInfo.collider.gameObject.name);

            int hitObjectLayer = hitInfo.collider.gameObject.layer;
            Debug.Log("Hit Object Layer: " + LayerMask.LayerToName(hitObjectLayer));

            int playerLayerMaskValue = playerLayerMask.value;
            Debug.Log("Player Layer Mask Value: " + playerLayerMaskValue);

            bool ignorePlayerLayer = (playerLayerMaskValue & (1 << hitObjectLayer)) != 0;
            Debug.Log("Ignore Player Layer: " + ignorePlayerLayer);

            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.time = maxLifetime;
            end = hitInfo.point;
            debugColor = Color.red;

          
            float maxDamage =damage; // Adjust this value as needed

            float damageMultiplier = 1.0f - distance / falloffRange;
            damageMultiplier = Mathf.Max(damageMultiplier, 0.0f);
             modifiedDamage = maxDamage * damageMultiplier;

            Debug.Log("Distance: " + distance);
            Debug.Log("Damage Multiplier: " + damageMultiplier);
            Debug.Log("Modified Damage: " + modifiedDamage);

            var hitbox = hitInfo.collider.GetComponent<Hitbox>();
            if (hitbox)
            {
                hitbox.onRaycastHit(this);
            }

            if (debug)
            {
                Debug.DrawLine(start, end, debugColor, 1.0f);
            }
        }
    }

    //Raycast segment with clamp
    //void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    //{
    //    Vector3 direction = end - start;
    //    float distance = direction.magnitude;
    //    Ray ray = new Ray(start, direction);

    //    Color debugColor = Color.green;
    //    int layerMask = ~playerLayerMask;
    //    if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, layerMask))
    //    {
    //        Debug.Log("Object Hit: " + hitInfo.collider.gameObject.name);

    //        int hitObjectLayer = hitInfo.collider.gameObject.layer;
    //        Debug.Log("Hit Object Layer: " + LayerMask.LayerToName(hitObjectLayer));

    //        int playerLayerMaskValue = playerLayerMask.value;
    //        Debug.Log("Player Layer Mask Value: " + playerLayerMaskValue);

    //        bool ignorePlayerLayer = (playerLayerMaskValue & (1 << hitObjectLayer)) != 0;
    //        Debug.Log("Ignore Player Layer: " + ignorePlayerLayer);

    //        hitEffect.transform.position = hitInfo.point;
    //        hitEffect.transform.forward = hitInfo.normal;
    //        hitEffect.Emit(1);

    //        bullet.time = maxLifetime;
    //        end = hitInfo.point;
    //        debugColor = Color.red;

    //        falloffRange = 5000.0f; // Adjust this value as needed
    //        float maxDamage = 100.0f; // Adjust this value as needed

    //        float damageMultiplier = 1.0f - distance / falloffRange;
    //        damageMultiplier = Mathf.Max(damageMultiplier, 0.0f);
    //        float modifiedDamage = maxDamage * damageMultiplier;

    //        Debug.Log("Distance: " + distance);
    //        Debug.Log("Damage Multiplier: " + damageMultiplier);
    //        Debug.Log("Modified Damage: " + modifiedDamage);

    //        var hitbox = hitInfo.collider.GetComponent<Hitbox>();
    //        if (hitbox)
    //        {
    //            hitbox.onRaycastHit(this);
    //        }

    //        if (debug)
    //        {
    //            Debug.DrawLine(start, end, debugColor, 1.0f);
    //        }
    //    }
    //}

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
    public void Restore(int totalAmmo_ref, int clipSize_ref)
    {
        totalAmmo = totalAmmo_ref;
        clipSize = clipSize_ref;
        ammoCount = clipSize;
    }

}