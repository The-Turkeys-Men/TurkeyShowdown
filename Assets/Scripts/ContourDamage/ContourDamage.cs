using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ContourDamage : MonoBehaviour
{
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] private Image _panelImage;
    Color _currentColor;

    [SerializeField, Range(0, 1)] private float _damageSpeed = 1;
    [SerializeField] private float _undamageSpeed = 1;
    
    void Start()
    {
        if (_panelImage != null)
        {
            _currentColor = _panelImage.color;
        }
        _healthComponent.OnDamaged.AddListener(Damage);
    }

    void Update()
    {
        if (_currentColor.a > 0)
        {
            UnDamage();
        }
    }
    void Damage()
    {
        _currentColor.a += Mathf.Clamp(_currentColor.a + _damageSpeed, 0, 1);
        _panelImage.color = _currentColor;
    }
    void UnDamage()
    {
        _currentColor.a-=Time.deltaTime * _undamageSpeed;
        _panelImage.color= _currentColor;
    }
}