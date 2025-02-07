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
            Debug.Log($"✅ [Client/Server] NetworkObject trouvé sur {gameObject.name}, isSpawned = {netObj.IsSpawned}");
            StartCoroutine(WaitAndSpawnRpc(netObj));
        }
        else
        {
            Debug.LogError($"❌ [Client/Server] NetworkObject MANQUANT sur {gameObject.name} !");
        }

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private IEnumerator WaitAndSpawnRpc(NetworkObject netObj)
    {
        yield return new WaitForSeconds(1f);
        
        if (IsServer && !netObj.IsSpawned)
        {
            netObj.Spawn();
            Debug.Log($"🚀 [Server] {gameObject.name} a été spawn !");
        }
    }

    private void OnServerStarted()
    {
        Debug.Log("Serveur démarré !");
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connecté : {clientId}");

        if (!testStarted)
        {
            testStarted = true;
            StartCoroutine(AddKillsWithDelayRpc());
        }
    }

    private IEnumerator AddKillsWithDelayRpc()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("🟡 [Client] Vérification : IsClient = " + IsClient + ", IsServer = " + IsServer);
        Debug.Log("⏳ [Client] Tentative d'envoi d'un kill au serveur...");
        ReportKillRpc("Ethan", "Bot", 1);
    }

    [Rpc(SendTo.Server)]
    private void ReportKillRpc(string killerName, string killedName, int weaponID)
    {
        Debug.Log($"🔴 [Server] ReportKillRpc reçu ! IsServer = {IsServer}, IsClient = {IsClient}");
        ReportKillToClientsRpc(killerName, killedName, weaponID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ReportKillToClientsRpc(string killerName, string killedName, int weaponID)
    {
        Debug.Log($"🔵 [Client] ReportKillToClientsRpc reçu : {killerName} a tué {killedName} avec l'arme {weaponID}");
    
        if (KillFeedManager.Instance != null)
        {
            KillFeedManager.Instance.AddKill(killerName, killedName, weaponID);
        }
        else
        {
            Debug.LogError("❌ KillFeedManager.Instance est NULL sur le client !");
        }
    }
}
