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
        HidePackClientRpc();
        component.Heal(_healAmount);
        DebuggerConsole.Instance.LogClientRpc("HealthPack grab on server");
        OnGrab.Invoke();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void HidePackClientRpc()
    {
        gameObject.SetActive(false);
        DebuggerConsole.Instance.Log("HealthPack grab on client");
    }

    public UnityEvent OnGrab { get; set; } = new();
}
