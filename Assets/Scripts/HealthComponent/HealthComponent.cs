using System;
using Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using WeaponSystem;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;
    public int BaseHealth;

    public NetworkVariable<int> Armor;
    public int MaxArmor;

    public UnityEvent<ulong> OnDeath;
    
    [SerializeField] private bool _isPlayer = false;

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
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SetHealthServerRpc(int health)
    {
        Health.Value = health;
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DamageServerRpc(int damage, ulong senderId)
    {
        Damage(damage, senderId);
    }

    public void Damage(int damage, ulong senderId)
    {
        GameObject senderObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[senderId].gameObject;
        if (senderObject.TryGetComponent(out TeamComponent senderTeamComponent) && TryGetComponent(out TeamComponent receiverTeamComponent))
        {
            if (senderTeamComponent.TeamID == receiverTeamComponent.TeamID)
            {
                return;
            }
        }
        
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
            if (_isPlayer)
            {
                //todo: optimize this
                FindAnyObjectByType<DeathMatchManager>().OnPlayerKill(senderId);
            }
        }
    }
}
