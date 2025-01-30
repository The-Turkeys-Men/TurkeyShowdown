using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private int _respawnTime = 5;
    [SerializeField] private Transform[] _playerSpawnPoint;
    
    private GameObject NewPlayer;
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void respawnPlayer(GameObject player)
    {
        player.SetActive(false);
        StartCoroutine(SpawnTimer(player));
    }

    IEnumerator SpawnTimer(GameObject player)
    {
        yield return new WaitForSeconds(_respawnTime);
        player.GetComponent<HealthComponent>().Health.Value = player.GetComponent<HealthComponent>().BaseHealth;
        player.transform.position = _playerSpawnPoint[Random.Range(0, _playerSpawnPoint.Length)].position;
        player.SetActive(true);
    }
    
    private void SpawnPlayer(ulong clientId)
    {
        if (IsServer)
        {
            NewPlayer = Instantiate(_playerPrefab, _playerSpawnPoint[Random.Range(0,_playerSpawnPoint.Length)].transform);
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
