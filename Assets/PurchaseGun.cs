using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseGun : MonoBehaviour
{
    public RaycastWeapon weaponFab;
    private ActiveWeapon activeWeapon;
    //check if it has activeWeapon script

    private void Start()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        
    }
    public void giveWeapon()
    {
        if (activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);

        }
    }
}




