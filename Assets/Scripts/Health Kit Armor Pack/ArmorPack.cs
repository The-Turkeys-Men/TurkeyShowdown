using Debugger;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ArmorPack : NetworkBehaviour, IGrabbable
{
    [SerializeField] private int _armorAmount = 25;
    [SerializeField] private float baseVolume;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<HealthComponent>(out var component)) return;
        
        if (!IsServer) return;
        gameObject.SetActive(false);
        component.ArmorUp(_armorAmount);
        DebuggerConsole.Instance.LogClientRpc("ArmorPack grab on server");
        OnGrab.Invoke();
        GetComponent<NetworkObject>().Despawn(true);
        AudioManager.Instance.PlaySFX("Armur",transform.position,baseVolume);
    }

    public UnityEvent OnGrab { get; set; } = new();
}
