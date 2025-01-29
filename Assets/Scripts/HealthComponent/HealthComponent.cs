using Unity.Netcode;
using UnityEngine.Events;

public class HealthComponent : NetworkBehaviour
{
    public NetworkVariable<int> Health;
    public int MaxHealth;

    public NetworkVariable<int> Armor;
    public int MaxArmor;
    
    UnityEvent OnDeath;

    public void Damage(int damage)
    {
        Health.Value -= damage;
        if (Health. Value <= 0)
        {
            OnDeath.Invoke();
        }
    }
}
