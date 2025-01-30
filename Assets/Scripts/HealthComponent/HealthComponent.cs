using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;

    public NetworkVariable<int> Armor;
    public int MaxArmor;

    public UnityEvent<GameObject> OnDeath;

    private void OnDeathServerRpcAttribute()
    {
        OnDeath.Invoke(this.gameObject);
    }
     
    private void OnDeathClientRpcAttribute()
    {
        OnDeath.Invoke(this.gameObject);
    }
    
    public void Damage(int damage)
    {
        Health.Value -= damage;
        if (Health. Value <= 0)
        {
            OnDeathServerRpcAttribute();
            OnDeathClientRpcAttribute();
        }
    }
}
