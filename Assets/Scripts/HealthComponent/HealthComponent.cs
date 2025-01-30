using System;
using Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;

    public NetworkVariable<int> Armor;
    public int MaxArmor;

    public UnityEvent<ulong> OnDeath;

    private void OnDeathServerRpcAttribute()
    {
        OnDeath.Invoke(gameObject.GetNetworkObjectId());
    }
     
    private void OnDeathClientRpcAttribute()
    {
        OnDeath.Invoke(gameObject.GetNetworkObjectId());
    }
    
    [ServerRpc]
    public void DamageServerRpc(int damage)
    {
        Health.Value -= damage;
        if (Health. Value <= 0)
        {
            OnDeathServerRpcAttribute();
            OnDeathClientRpcAttribute();
        }
    }
}
