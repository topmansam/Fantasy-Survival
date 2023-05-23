using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public Health health;
   public void onRaycastHit(RaycastWeapon weapon, Vector3 direction)
    {
        health.TakeDamage(weapon.damage, direction);
    }
}
