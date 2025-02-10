using Debugger;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HealthPack : NetworkBehaviour, IGrabbable
{
    [SerializeField] private int _healAmount = 25;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<HealthComponent>(out var component)) return;
        
        if (!IsServer) return;
        gameObject.SetActive(false);
        component.Heal(_healAmount);
        DebuggerConsole.Instance.LogClientRpc("HealthPack grab on server");
        OnGrab.Invoke();
        GetComponent<NetworkObject>().Despawn(true);
        AudioManager.Instance.PlaySFX("soin",transform.position);
    }

    public UnityEvent OnGrab { get; set; } = new();
}
