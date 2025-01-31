using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Netcode;
using UnityEngine;

public class Playerweapon : NetworkBehaviour 
{
     GameObject closestWeapon ;
      public float WeaponGrabRange;
      public BaseWoapon EquipedWeapon;
      public List<BaseWoapon>WapontInventari;
      public bool Dizziness;
      

      
      public GameObject firstWeapon;
      void Start()
      {
        EquipedWeapon.Equipped=false;
        
      }

    // Update is called once per frame
    void Update()
    {
        tryShoot();
        TryThrowWeapon();
        EquipedNewWeapon();
        UnequipWeapon();
       
        Debug. Log(closestWeapon);


    }
    public void tryShoot()
    {
        if(Input.GetKeyUp(KeyCode.K))
        {
            if(Dizziness!=true)
        {
            
            Shoot();
        }
        }
        

    }
    public void TryThrowWeapon()
    {
        if(Input.GetKeyUp(KeyCode.L))
        {
        ThrowWeapon();
        closestWeapon.transform .SetParent(null);
        closestWeapon=null;
        Debug. Log(closestWeapon);
        }
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
            closestWeapon = null;
            Collider2D[] weaponColliders = Physics2D.OverlapCircleAll(transform.position, WeaponGrabRange,  1 << LayerMask.NameToLayer("Machin"));
            Debug.Log(weaponColliders.Length);
            if (weaponColliders.Length > 0) 
           {
                closestWeapon = null;
                float distance = float.MaxValue;
                foreach (Collider2D weaponCollider in weaponColliders) 
                {
                    float currentDistance = Vector3.Distance(transform.position, weaponCollider.transform.position);
                    if (currentDistance < distance)
                    {
                        distance = currentDistance;
                        closestWeapon = weaponCollider.gameObject;
                    }
                }
                closestWeapon.transform .SetParent(transform);
                closestWeapon.transform.position=transform.position;
                EquipedWeapon.Equipped=true;
                    
                firstWeapon.SetActive(false);
                EquipedWeapon.PickUp=false;
                
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
