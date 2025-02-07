using Unity.Netcode;
using UnityEngine;

public class PlayerKillHandler : NetworkBehaviour
{
    public static PlayerKillHandler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [ServerRpc]
    public void ReportKillServerRpc(ulong killerId, ulong killedId, int weaponId)
    {
        ReportKillClientRpc(killerId, killedId, weaponId);
    }

    [ClientRpc]
    private void ReportKillClientRpc(ulong killerId, ulong killedId, int weaponId)
    {
        string killerName = $"Player {killerId}";
        string killedName = $"Player {killedId}";
        KillFeedManager.Instance.AddKill(killerName, killedName, weaponId);
    }
}
