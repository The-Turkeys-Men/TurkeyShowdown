using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthComponent _healthComponent;
    
    [SerializeField] private Image _healthBar;

    private void Awake()
    {
        if (!_healthComponent) _healthComponent = GetComponentInParent<HealthComponent>();
    }

    private void Update()
    {
        _healthBar.fillAmount = (float)_healthComponent.Health.Value / _healthComponent.MaxHealth;
    }
}
