using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class KillFeedTest : NetworkBehaviour
{
    private bool testStarted = false;

    private void Start()
    {
        if (TryGetComponent(out NetworkObject netObj))
        {
            StartCoroutine(WaitAndSpawnRpc(netObj));
        }

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private IEnumerator WaitAndSpawnRpc(NetworkObject netObj)
    {
        yield return new WaitForSeconds(5f);
        
        if (IsServer && !netObj.IsSpawned)
        {
            netObj.Spawn();
        }
    }

    private void OnServerStarted()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!testStarted)
        {
            testStarted = true;
            StartCoroutine(AddKillsWithDelayRpc());
        }
    }

    private IEnumerator AddKillsWithDelayRpc()
    {
        yield return new WaitForSeconds(2f);
        KillFeedManager.Instance.AddKillServerRpc("Ethan", "Bot", 1);
    }

    [Rpc(SendTo.Server)]
    private void ReportKillRpc(string killerName, string killedName, int weaponID)
    {
        ReportKillToClientsRpc(killerName, killedName, weaponID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ReportKillToClientsRpc(string killerName, string killedName, int weaponID)
    {
        if (KillFeedManager.Instance != null)
        {
            KillFeedManager.Instance.AddKill(killerName, killedName, weaponID);
        }
    }
}