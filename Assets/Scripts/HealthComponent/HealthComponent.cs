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

    public UnityEvent<GameObject> OnDeath;

    private void OnDeathServerRpcAttribute()
    {
        OnDeath.Invoke(this.gameObject);
    }
     
    private void OnDeathClientRpcAttribute()
    {
        OnDeath.Invoke(this.gameObject);
    }

    public void Heal(int amount)
    {
        Health.Value = Mathf.Clamp((Health.Value + amount), 0, MaxHealth);
    }

    public void ArmorUp(int amount)
    {
        Armor.Value = Mathf.Clamp(Armor.Value + amount, 0, MaxArmor);
    }
    
    public void Damage(int damage)
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
