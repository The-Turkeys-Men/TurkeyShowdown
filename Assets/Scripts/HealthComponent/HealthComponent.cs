using System;
using Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;
    public int BaseHealth;

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

    public void Heal(int amount)
    {
        Health.Value = Mathf.Clamp((Health.Value + amount), 0, MaxHealth);
    }

    public void ArmorUp(int amount)
    {
        Armor.Value = Mathf.Clamp(Armor.Value + amount, 0, MaxArmor);
    }
    
    [ServerRpc]
    public void DamageServerRpc(int damage)
    {
        if (Armor.Value > 0)
        {
            Armor.Value -= damage;
            if (Armor.Value <= 0)
            {
                Health.Value += Armor.Value;
                Armor.Value = 0;
            }
        }
        else
        {
            Health.Value -= damage;
        }
        if (Health. Value <= 0)
        {
            OnDeathServerRpcAttribute();
            OnDeathClientRpcAttribute();
        }
    }
}
