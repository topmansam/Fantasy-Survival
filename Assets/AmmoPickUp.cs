using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        // Check if it has ActiveWeapon script
        ActiveWeapon activeWeapon = other.gameObject.GetComponentInChildren<ActiveWeapon>();

        if (activeWeapon)
        {
            // Refill both primary and secondary weapons
            RaycastWeapon primaryWeapon = activeWeapon.GetWeapon((int)ActiveWeapon.WeaponSlot.Primary);
            if (primaryWeapon)
            {
                primaryWeapon.Restore(30,15);
                Debug.Log("Refreshing ammo for primary weapon");
            }

            RaycastWeapon secondaryWeapon = activeWeapon.GetWeapon((int)ActiveWeapon.WeaponSlot.Secondary);
            if (secondaryWeapon)
            {
                secondaryWeapon.Restore(24,8);
                Debug.Log("Refreshing ammo for secondary weapon");
            }
        }
    }
}
