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
    public void ReportKillServerRpc(ulong killerID, ulong killedID, int weaponID)
    {
        ReportKillClientRpc(killerID, killedID, weaponID);
    }

    [ClientRpc]
    private void ReportKillClientRpc(ulong killerID, ulong killedID, int weaponID)
    {
        string killerName = $"Player {killerID}";
        string killedName = $"Player {killedID}";
        KillFeedManager.Instance.AddKill(killerName, killedName, weaponID);
    }
}
