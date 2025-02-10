using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private int _respawnTime = 5;
    [SerializeField] private Transform[] _playerSpawnPoint;
    private GameObject NewPlayer;
    public static PlayerSpawner SpawnerInstance;

    [SerializeField] private BaseWeapon _spawnWeapon;
    
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void Awake()
    {
        if (SpawnerInstance == null)
        { 
            SpawnerInstance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Respawn

    /*[Rpc(SendTo.ClientsAndHost)]
    private void OnDeathClientRpc(ulong playerObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var playerObject))
        {
            Debug.LogError("Failed to get player object");
            return;
        }
        
        //playerObject.gameObject.SetActive(false);
       // StartCoroutine(SpawnTimer(playerObject.gameObject));
    }
    
    [Rpc(SendTo.Server)]
    private void OnDeathServerRpc(ulong playerObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var playerObject))
        {
            Debug.LogError("Failed to get player object");
            return;
        }
        
        //playerObject.gameObject.SetActive(false);
        // StartCoroutine(SpawnTimer(playerObject.gameObject));
    }
    
   IEnumerator SpawnTimer(GameObject player)
    {
        yield return new WaitForSeconds(_respawnTime);
        RespawnPlayer(player);
    }*/

    public void RespawnPlayer(GameObject player)
    {
        var healthComponent = player.GetComponent<HealthComponent>();
        healthComponent.SetHealthServerRpc(healthComponent.BaseHealth);
        
        //Check Player Around Spawns
        List<Transform> emptySpawns = new List<Transform>();
        int minCount = int.MaxValue;
        
        foreach (var checkSpawn in _playerSpawnPoint)
        {
            var colliders = Physics2D.OverlapCircleAll(checkSpawn.position, 10f, 1 << LayerMask.NameToLayer("Player"));
            int count = colliders.Length;
            
            //Clears the list if a spawner is no longer isolated
            if (count < minCount)
            {
                minCount = count;
                emptySpawns.Clear();
                emptySpawns.Add(checkSpawn);
            }
            else if (count == minCount)
            {
                emptySpawns.Add(checkSpawn);
            }
        }
        
        var spawn = emptySpawns.PickRandom();
        player.transform.position = spawn.position;
        
        player.SetActive(true);
        healthComponent.OnRespawn.Invoke();
        
        /*if (_spawnWeapon)
        {
            BaseWeapon newWeapon = Instantiate(_spawnWeapon, NewPlayer.transform.position, Quaternion.identity);
            newWeapon.GetComponent<NetworkObject>().Spawn();
            NewPlayer.GetComponent<PlayerWeapon>().EquipWeapon(newWeapon);
        }*/
        
        OnFinishRespawnClientRpc(player.GetNetworkObjectId());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnFinishRespawnClientRpc(ulong playerObjectId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var playerObject))
        {
            Debug.LogError("Failed to get player object");
            return;
        }

        var networkTransform = playerObject.GetComponent<NetworkTransform>();
        networkTransform.Interpolate = false;
        playerObject.transform.position = _playerSpawnPoint[Random.Range(0, _playerSpawnPoint.Length)].position;
        playerObject.gameObject.SetActive(true);
        
        var healthComponent = playerObject.GetComponent<HealthComponent>();
        healthComponent.OnRespawn.Invoke();
        
        StartCoroutine(ReactivateInterpolation(networkTransform));
    }

    private IEnumerator ReactivateInterpolation(NetworkTransform networkTransform)
    {
        yield return new WaitForSeconds(0.2f);
        networkTransform.Interpolate = true;
    }

    #endregion
    
    
    
    private void SpawnPlayer(ulong clientId)
    {
        if (!IsServer)
        {
            return;
        }
        
        NewPlayer = Instantiate(_playerPrefab, _playerSpawnPoint[Random.Range(0,_playerSpawnPoint.Length)].transform);
        NewPlayer.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        NewPlayer.GetComponent<HealthComponent>().OnDeath.AddListener((playerObjectId) =>
        {
            //OnDeathClientRpc(playerObjectId);
            //OnDeathServerRpc(playerObjectId);

        });
        

        if (_spawnWeapon)
        {
            BaseWeapon newWeapon = Instantiate(_spawnWeapon, NewPlayer.transform.position, Quaternion.identity);
            newWeapon.GetComponent<NetworkObject>().Spawn();
            NewPlayer.GetComponent<PlayerWeapon>().EquipWeapon(newWeapon);
            MakeThePlayerEquipWeaponRpc(NewPlayer.GetNetworkObjectId(), newWeapon.NetworkObjectId, 
                RpcTarget.Single(NewPlayer.GetComponent<NetworkObject>().OwnerClientId, RpcTargetUse.Temp));
        }

        ActivateCameraClientRpc(NewPlayer.GetComponent<NetworkObject>().NetworkObjectId,
            RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }
    
    [Rpc(SendTo.ClientsAndHost, AllowTargetOverride = true)]
    private void ActivateCameraClientRpc(ulong playerId, RpcParams rpcParams = default)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var playerObject);
        playerObject.GetComponentInChildren<Camera>(true).gameObject.SetActive(true);
        //playerObject.GetComponentInChildren<AudioListener>().enabled = true;
    }

    [Rpc(SendTo.SpecifiedInParams, AllowTargetOverride = true)]
    private void MakeThePlayerEquipWeaponRpc(ulong playerObjectId, ulong weaponObjectId, RpcParams rpcParams)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var playerObject);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(weaponObjectId, out var weaponObject);
        playerObject.GetComponent<PlayerWeapon>().EquipWeapon(weaponObject.GetComponent<BaseWeapon>());
    }
}
