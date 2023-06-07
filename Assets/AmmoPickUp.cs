using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");
        //check if it has activeWeapon script
        RaycastWeapon weapon = other.gameObject.GetComponentInChildren<RaycastWeapon>();

        if (weapon)
        {
            weapon.Restore();
            Debug.Log("refreshig ammo");
        }

    }
}
