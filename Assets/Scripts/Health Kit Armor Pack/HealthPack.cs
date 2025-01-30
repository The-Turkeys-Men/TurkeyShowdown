using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] private int _healAmount = 25;
    [SerializeField] private int _maxHealth = 100;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<HealthComponent>())
        {
            HealthComponent healthComponent = other.GetComponent<HealthComponent>();
            if (healthComponent.Health.Value <= _maxHealth)
            {
                healthComponent.Health.Value += _healAmount;
                if (healthComponent.Health.Value > _maxHealth)
                {
                    healthComponent.Health.Value = _maxHealth;
                }
            }
        }
    }
}
