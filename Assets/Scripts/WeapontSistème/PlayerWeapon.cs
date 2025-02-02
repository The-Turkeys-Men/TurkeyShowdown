using System;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerWeapon : NetworkBehaviour 
{
    public float WeaponGrabRange;
    public List<BaseWeapon>WeaponInventory;
    public BaseWeapon EquipedWeapon;

    public Transform WeaponHolder;
    
    public bool Dizziness;

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (EquipedWeapon)
        {
            EquipedWeapon.transform.position = WeaponHolder.position;
            EquipedWeapon.transform.rotation = WeaponHolder.rotation;
        }
    }
    public void tryShoot()
    {
        if (!EquipedWeapon || Dizziness)
        {
            return;
        }
        
        if (EquipedWeapon.Ammo.Value <= 0)
        {
            TryThrowWeapon();
        }
        else
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
        EquipedWeapon.Rb.simulated = true;
        EquipedWeapon.Rb.linearVelocity= transform.TransformDirection(Vector3.forward) * EquipedWeapon.lance;
        
        ulong weaponId = EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId;
        OnThrowWeaponServerRpc(weaponId);
        UnEquipWeapon();
    }

    [ServerRpc]
    private void OnThrowWeaponServerRpc(ulong weaponObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out NetworkObject weaponNetworkObject);
        var weaponComponent = weaponNetworkObject.GetComponent<BaseWeapon>();
        weaponComponent.IsThrowed.Value = true;
        weaponComponent.CanBePickUp.Value = true;
        weaponNetworkObject.TrySetParent((Transform)null);
        weaponNetworkObject.RemoveOwnership();
    }

    private void UnEquipWeapon()
    {
        if (WeaponInventory.Contains(EquipedWeapon))
        {
            WeaponInventory.Remove(EquipedWeapon);
        }
        EquipedWeapon = null;
        if (WeaponInventory.Count > 0)
        {
            EquipedWeapon = WeaponInventory[0];
        }
    }
    
    public void TryEquipWeapon()
    {
        GameObject closestWeapon = null;
        Collider2D[] weaponColliders = Physics2D.OverlapCircleAll(transform.position, WeaponGrabRange,  1 << LayerMask.NameToLayer("Weapon"));
        
        float distance = float.MaxValue;
        foreach (Collider2D weaponCollider in weaponColliders) 
        {
            if (weaponCollider.GetComponent<BaseWeapon>().CanBePickUp.Value == false)
            {
                continue;
            }
            
            float currentDistance = Vector3.Distance(transform.position, weaponCollider.transform.position);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closestWeapon = weaponCollider.gameObject;
            }
        }
        
        if (!closestWeapon)
        {
            return;
        }

        EquipedWeapon = closestWeapon.GetComponent<BaseWeapon>();
        OnEquipWeaponServerRpc(OwnerClientId, GetComponent<NetworkObject>().NetworkObjectId, EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
        AskForOwnershipServerRpc(OwnerClientId, EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
        EquipedWeapon.GetComponent<Rigidbody2D>().simulated = false;
        EquipedWeapon.transform.position = transform.position + Vector3.up;
    }
    
    [ServerRpc]
    private void OnEquipWeaponServerRpc(ulong clientId, ulong playerObjectId, ulong weaponObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out NetworkObject playerNetworkObject);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out NetworkObject weaponNetworkObject);

        weaponNetworkObject.TrySetParent(playerNetworkObject);
        weaponNetworkObject.GetComponent<BaseWeapon>().CanBePickUp.Value = false;
    }

    [ServerRpc]
    private void AskForOwnershipServerRpc(ulong clientId, ulong networkObjectId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        networkObject.ChangeOwnership(clientId);
    }
}
