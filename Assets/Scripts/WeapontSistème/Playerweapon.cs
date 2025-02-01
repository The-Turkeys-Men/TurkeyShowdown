using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Playerweapon : NetworkBehaviour 
{
    public float WeaponGrabRange;
    public List<BaseWeapon>WeaponInventory;
    public BaseWeapon EquipedWeapon;

    public bool Dizziness;

    public GameObject firstWeapon;

    void Update()
    {
        //tryShoot();
        //TryThrowWeapon();
        //EquipedNewWeapon();
    }
    public void tryShoot()
    {
        if (Dizziness != true)
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
        GameObject closestWeapon = null;
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
                
            firstWeapon.SetActive(false);
            EquipedWeapon.PickUp=false;
        }
    }
}
