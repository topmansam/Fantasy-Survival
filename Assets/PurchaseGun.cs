using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseGun : MonoBehaviour
{
    public RaycastWeapon weaponFab;
 
    //check if it has activeWeapon script

    
    public void GiveWeapon()
    {
        ActiveWeapon activeWeapon = GetComponent<ActiveWeapon>();
        if (activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);

        }
    }
}




