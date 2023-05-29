using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator rigController;
    public WeaponAnimationEvents animationEvents;
    public ActiveWeapon activeWeapon;
    public Transform leftHand;
    public AmmoWidget ammoWidget;
    public bool isReloading;
    int refilAmmo;
    GameObject magazineHand;

    // Start is called before the first frame update
    void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            ammoWidget.Refresh(weapon.ammoCount, weapon.totalAmmo);
            // we check this twice for when we are on our last mag, we still want the text to refresh
            if (weapon.isFiring)
            {
                ammoWidget.Refresh(weapon.ammoCount, weapon.totalAmmo);
            }
            
            //Dont reload if we have no more total ammo left

            if (weapon.totalAmmo <= 0) return;

            // If we press the R key or we just have no more shots to fire, then reload.
                if (Input.GetKeyDown(KeyCode.R) || weapon.ammoCount <= 0)
            {
              
                isReloading = true;
                rigController.SetTrigger("reload_weapon");
                
            }

            if (weapon.isFiring)
            {
                ammoWidget.Refresh(weapon.ammoCount, weapon.totalAmmo);
            }
        }
    }

    void OnAnimationEvent(string eventName)
    {
        Debug.Log(eventName);
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }

    void DetachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        magazineHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }

    void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        Rigidbody body = droppedMagazine.AddComponent<Rigidbody>();
        body.AddForce(new Vector3(-0.5f, 1, 0) * 0.5f, ForceMode.Impulse);
        //droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
    }

    void RefillMagazine()
    {
        magazineHand.SetActive(true);
    }

    void AttachMagazine()
    {
        RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magazineHand);
       
        
        //This is the amount of ammo we need to refill
         refilAmmo = weapon.clipSize - weapon.ammoCount;

        // if the amount of ammo we need is greater than the total amount of ammmo we have available
        if (refilAmmo > weapon.totalAmmo)
        {
            // set the amount of ammo we will refill to how much ammo we have currently plus the total ammount of ammo avaiabe
            refilAmmo = weapon.totalAmmo+weapon.ammoCount;
            // set the clip size to the amount of ammo we just refiled
            weapon.clipSize = refilAmmo;
            // set the current ammo to the current clip
            weapon.ammoCount = weapon.clipSize;
            //set the total ammo left to 0 as we just used up everything
            weapon.totalAmmo = 0;
        }
        else
        {
            //if the amount of ammo we need is greater or equal to our total amount of avaialbe ammo, subtract the refil ammo from the total avaialble ammo
            weapon.totalAmmo -= refilAmmo;
           
            //set the amount of ammo we refilled to the clip size and to our current ammmo. This will just be our regular clip size.
            weapon.ammoCount = weapon.clipSize;
        }
        //weapon.clipSize = refilAmmo;
        rigController.ResetTrigger("reload_weapon");
        ammoWidget.Refresh(weapon.ammoCount, weapon.totalAmmo);
        isReloading = false;
    }
}