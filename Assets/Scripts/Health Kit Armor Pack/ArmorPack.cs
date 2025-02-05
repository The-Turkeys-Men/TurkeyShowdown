using Debugger;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ArmorPack : NetworkBehaviour, IGrabbable
{
    [SerializeField] private int _armorAmount = 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<HealthComponent>(out var component)) return;
        
        if (!IsServer) return;
        component.ArmorUp(_armorAmount);
        HidePackClientRpc();
        DebuggerConsole.Instance.LogClientRpc("ArmorPack grab on server");
        OnGrab.Invoke();
    }
    
    [ClientRpc(RequireOwnership = false)]
    private void HidePackClientRpc()
    {
        gameObject.SetActive(false);
        DebuggerConsole.Instance.Log("ArmorPack grab on client");
    }

    public UnityEvent OnGrab { get; set; } = new();
}
