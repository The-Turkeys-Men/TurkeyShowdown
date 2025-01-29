using Unity.Netcode;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;

    public NetworkVariable<int> Armor;
    public int MaxArmor;
    
    UnityEvent OnDeath;

    [ServerRpc]
    private void OnDeathServerRpc()
    {
        OnDeath.Invoke();
    }

    [ClientRpc]
    private void OnDeathClientRpc()
    {
        OnDeath.Invoke();
    }
    
    public void Damage(int damage)
    {
        Health.Value -= damage;
        if (Health. Value <= 0)
        {
            OnDeathServerRpc();
            OnDeathClientRpc();
        }
    }
}
