using UnityEngine;

public class ShotgunWeapon : RaycastWeapon
{
    private void FireShotgun()
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

        Vector3 forward = (raycastDestination.position - raycastOrigin.position).normalized;
        Vector3 right = raycastOrigin.right;
        Vector3 up = raycastOrigin.up;

        // Calculate the spread angles for the three bullets (adjust these values for desired spread)
        float spreadAngle = 2.0f; // Total spread angle
        float individualSpread = spreadAngle / 2.0f; // Individual spread angle for each bullet

        // Calculate the spread directions for the three bullets
        Vector3 spreadDirection1 = Quaternion.AngleAxis(-individualSpread, up) * forward;
        Vector3 spreadDirection2 = forward;
        Vector3 spreadDirection3 = Quaternion.AngleAxis(individualSpread, up) * forward;

        // Calculate the velocities for the three bullets
        Vector3 velocity1 = spreadDirection1 * bulletSpeed;
        Vector3 velocity2 = spreadDirection2 * bulletSpeed;
        Vector3 velocity3 = spreadDirection3 * bulletSpeed;

        // Create and add the three bullets
        var bullet1 = CreateBullet(raycastOrigin.position, velocity1);
        var bullet2 = CreateBullet(raycastOrigin.position, velocity2);
        var bullet3 = CreateBullet(raycastOrigin.position, velocity3);
        bullets.Add(bullet1);
        bullets.Add(bullet2);
        bullets.Add(bullet3);

        recoil.GenerateRecoil(weaponName);
        ammoWidget.Refresh(ammoCount, totalAmmo);
    }

    public override void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0.0f)
        {
            FireShotgun();
            accumulatedTime -= fireInterval;
        }
    }
}