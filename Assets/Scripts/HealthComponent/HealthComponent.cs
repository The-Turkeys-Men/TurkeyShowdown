using Debugger;
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

    [HideInInspector] public UnityEvent<ulong> OnDeath = new();
    [HideInInspector] public UnityEvent OnRespawn = new();
    [HideInInspector] public UnityEvent OnDamaged = new();
    public ParticleSystem particleSystemDamage;
    
    
    [SerializeField] private bool _isPlayer = false;
     
     [SerializeField] private float baseVolume;
    public bool IsDead => Health.Value <= 0;
    
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void OnDeathClientRpc()
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
        if (IsDead)
        {
            return;
        }
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
            particleSystemDamage.maxParticles=10;
            particleSystemDamage.Play();
            Armor.Value -= damage;
            AudioManager.Instance.PlaySFX("crisDinde",transform.position,baseVolume);
            if (Armor.Value <= 0)
            {
                Health.Value += Armor.Value;
                Armor.Value = 0;
            }
        }
        else
        {
            Health.Value -= damage;
            PlayDamageParticleClientRpc(10);
            AudioManager.Instance.PlaySFX("crisDinde",transform.position,baseVolume);
        }
        if (Health.Value <= 0)
        {
            PlayDamageParticleClientRpc(30);
            AudioManager.Instance.PlaySFX("mort",transform.position,baseVolume);
            OnDeath.Invoke(NetworkObjectId);
            OnDeathClientRpc();
            if (_isPlayer)
            {
                var killerId = senderObject.GetComponent<NetworkObject>().OwnerClientId;
                ((DeathMatchManager)DeathMatchManager.GetInstance()).OnPlayerKill(killerId);
                DebuggerConsole.Instance.LogClientRpc("Player killed by: " + senderObject.name);
            }
        }
        OnDamaged.Invoke();
        OnDamagedClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayDamageParticleClientRpc(int amount)
    {
        particleSystemDamage.maxParticles=30;
        particleSystemDamage.Play();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void OnDamagedClientRpc()
    {
        OnDamaged.Invoke();
    }
}
