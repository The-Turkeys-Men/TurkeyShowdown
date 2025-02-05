using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

    [Rpc(SendTo.Server)]
    public void UpdateWeaponPosServerRpc(ulong weaponId, Vector3 setPos)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponId, out NetworkObject weaponNetworkObject);
        weaponNetworkObject.transform.position = setPos;
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
        
        ulong weaponId = EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId;
        UpdateWeaponPosServerRpc(weaponId, WeaponHolder.position);
        OnThrowWeaponServerRpc(weaponId, EquipedWeapon.transform.right);
        UnEquipWeapon();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void OnThrowWeaponServerRpc(ulong weaponObjectId, Vector2 direction)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out NetworkObject weaponNetworkObject);
        weaponNetworkObject.TrySetParent((Transform)null);
        weaponNetworkObject.RemoveOwnership();
        
        var weaponComponent = weaponNetworkObject.GetComponent<BaseWeapon>();
        weaponComponent.IsThrowed.Value = true;
        
        weaponComponent.Rb.simulated = true;
        weaponComponent.Rb.AddForce(direction.normalized * weaponComponent.ThrowForce, ForceMode2D.Impulse);
        weaponComponent.Rb.AddTorque(weaponComponent.ThrowTorque, ForceMode2D.Impulse);
        weaponComponent.ShowClientRpc();
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
            BaseWeapon weaponComponent = weaponCollider.GetComponent<BaseWeapon>();
            if (weaponComponent.CanBePickUp.Value == false)
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
        EquipedWeapon.LastOwner = gameObject;
        EquipedWeapon.ShootPoint = WeaponHolder;
        OnEquipWeaponServerRpc(OwnerClientId, GetComponent<NetworkObject>().NetworkObjectId, EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
        AskForOwnershipServerRpc(OwnerClientId, EquipedWeapon.GetComponent<NetworkObject>().NetworkObjectId);
        EquipedWeapon.GetComponent<Rigidbody2D>().simulated = false;
        EquipedWeapon.transform.position = transform.position + Vector3.up;
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void OnEquipWeaponServerRpc(ulong clientId, ulong playerObjectId, ulong weaponObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out NetworkObject playerNetworkObject);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out NetworkObject weaponNetworkObject);

        weaponNetworkObject.TrySetParent(playerNetworkObject);
        var baseWeapon = weaponNetworkObject.GetComponent<BaseWeapon>();
        baseWeapon.CanBePickUp.Value = false;
        baseWeapon.HideClientRpc();
        baseWeapon.GetComponent<IGrabbable>().OnGrab.Invoke();
        baseWeapon.LastOwner = playerNetworkObject.gameObject;
        
        OnEquipWeaponClientRpc(playerObjectId, weaponObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnEquipWeaponClientRpc( ulong playerObjectId, ulong weaponObjectId)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out NetworkObject playerNetworkObject);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out NetworkObject weaponNetworkObject);
        
        var baseWeapon = weaponNetworkObject.GetComponent<BaseWeapon>();
        baseWeapon.ShootPoint = playerNetworkObject.GetComponent<PlayerWeapon>().WeaponHolder;
    }

    [Rpc(SendTo.Server)]
    private void AskForOwnershipServerRpc(ulong clientId, ulong networkObjectId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        networkObject.ChangeOwnership(clientId);
    }
}
