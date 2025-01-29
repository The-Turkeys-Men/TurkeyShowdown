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

    private void SpawnPlayer(ulong clientId)
    {
        if (IsServer)
        {
            NewPlayer = Instantiate(playerPrefab, playerSpawnPoint[Random.Range(0,playerSpawnPoint.Length)].transform);
            NewPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
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
