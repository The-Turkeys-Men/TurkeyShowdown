using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Netcode;
using UnityEngine;

public class Playerweapon : NetworkBehaviour 
{
      public float WeaponGrabRange;
      public BaseWoapon EquipedWeapon;
      public List<BaseWoapon>WapontInventari;
      public bool Dizziness;
      
      public GameObject firstWeapon;

    // Update is called once per frame
    void Update()
    {
        tryShoot();
        TryThrowWeapon();
        EquipedNewWeapon();
        UnequipWeapon();

    }
    public void tryShoot()
    {
        if(Dizziness!=true)
        {
            
            Shoot();
        }

    }
    public void TryThrowWeapon()
    {
        ThrowWeapon();
    }
    public void Shoot()
    {
        EquipedWeapon.tryShoot();
    }
    public void ThrowWeapon()
    {
        EquipedWeapon.TryThrowWeapon();
    }
    public void EquipedNewWeapon()
    {
        if (Input.GetKey(KeyCode.F)) //sur le script PlayerWeapons
        {
            Collider2D[] weaponColliders = Physics2D.OverlapCircleAll(transform.position, 3,  1 << LayerMask.NameToLayer("Weapon"));
            if (weaponColliders.Length > 0) 
           {
                BaseWoapon closestWeapon = null;
                float distance = float.MaxValue;
                foreach (Collider2D weaponCollider in weaponColliders) 
                {
                    float currentDistance = Vector3.Distance(transform.position, weaponCollider.transform.position);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestWeapon = weaponCollider.GetComponent<BaseWoapon>();
                    }
                }
                closestWeapon.transform.SetParent(transform);
                    
                firstWeapon.SetActive(false);
                EquipedWeapon.PickUp=false;
                EquipedWeapon.Equipped=true;
           }
            
        }
    }
    public void UnequipWeapon()
    {
        if(EquipedWeapon.Equipped==false)
        {
            firstWeapon.SetActive(true);
        }
        

    }

}






