using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] playerSpawnPoint;
    
    private GameObject NewPlayer;
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void respawnPlayer(GameObject player)
    {
        player.SetActive(false);
        player.GetComponent<HealthComponent>().Health.Value = player.GetComponent<HealthComponent>().MaxHealth;
        player.transform.position = playerSpawnPoint[Random.Range(0, playerSpawnPoint.Length)].position;
        player.SetActive(true);
    }
    
    
    private void SpawnPlayer(ulong clientId)
    {
        if (IsServer)
        {
            NewPlayer = Instantiate(playerPrefab, playerSpawnPoint[Random.Range(0,playerSpawnPoint.Length)].transform);
            NewPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            NewPlayer.GetComponent<HealthComponent>().OnDeath.AddListener(respawnPlayer);
            ClientRpcParams clientRpcParams = new()
            {
                Send = new()
                {
                    TargetClientIds = new[] { clientId }
                }
            };
            ActivateCameraClientRpc(NewPlayer.GetComponent<NetworkObject>().NetworkObjectId,clientRpcParams);
        }
    }

    [ClientRpc]
    private void ActivateCameraClientRpc(ulong playerId, ClientRpcParams clientRpcParams = default)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var playerObject);
        playerObject.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        //playerObject.GetComponentInChildren<AudioListener>().enabled = true;
    }
}
