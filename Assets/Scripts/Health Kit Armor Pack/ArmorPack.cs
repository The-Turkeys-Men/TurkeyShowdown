using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ArmorPack : NetworkBehaviour, IGrabbable
{
    [SerializeField] private int _armorAmount = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<HealthComponent>(out var component)) return;
        gameObject.SetActive(false);
        if (!IsServer) return;
        component.ArmorUp(_armorAmount);
        OnGrab.Invoke();
    }

    public UnityEvent OnGrab { get; set; } = new();
}
