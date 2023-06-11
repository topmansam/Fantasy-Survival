using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health health;
   
   public void onRaycastHit(RaycastWeapon weapon)
    {

        health.TakeDamage(weapon.modifiedDamage);
        Debug.Log("took " + weapon.modifiedDamage);
    }
}
