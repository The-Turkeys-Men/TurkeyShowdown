using Unity.Netcode;
using UnityEngine;

public class KnockbackHandler : NetworkBehaviour
{
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void ApplyKnockbackServerRpc(Vector2 direction, float force)
    {
        ApplyKnockbackClientRpc(direction, force);
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    public void ApplyKnockbackClientRpc(Vector2 direction, float force)
    {
        GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
    }
}
