using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TVLighting : MonoBehaviour
{
    [SerializeField] private Light2D _light;

    private float _minValue = 0.3f;
    private float _maxValue = 0.5f;
    private float _pulseSpeed = 50f;

    private void Start()
    {
        _light = GetComponent<Light2D>();
    }

    void Update()
    {
        if (_light == null) { return; }
        float intensity = Mathf.Lerp(_minValue, _maxValue, (Mathf.Sin(Time.time *_pulseSpeed) + 1) / 2);
        _light.intensity = intensity;

    }
}
