using UnityEngine;
using UnityEngine.UI;

public class ArmorBar : MonoBehaviour
{
    [SerializeField] private HealthComponent _healthComponent;
    
    [SerializeField] private Image _armorBar;

    private void Awake()
    {
        if (!_healthComponent) _healthComponent = GetComponentInParent<HealthComponent>();
    }

    private void Update()
    {
        _armorBar.fillAmount = (float)_healthComponent.Armor.Value / _healthComponent.MaxArmor;
    }
}
