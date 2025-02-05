using System.Collections;
using System.Collections.Generic;
using Extensions;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private List<GameObject> _Item;
    [SerializeField] private int _respawnTime = 30;
    
    private GameObject _spawnedItem;

    [SerializeField] private bool _reinstantiate = false;

    private void Initialize()
    {
        if (!IsServer) return;
        SpawnItem();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

    private void StartRespawn()
    {
        StartCoroutine(RespawnItems());
    }

    IEnumerator RespawnItems()
    {
        if (_spawnedItem && _spawnedItem.TryGetComponent(out IGrabbable grabbable))
        {
             grabbable.OnGrab.RemoveListener(StartRespawn);
        }
        yield return new WaitForSeconds(_respawnTime);
        if (!_spawnedItem || _reinstantiate)
        {
            SpawnItem();
        }
        MakeItemVisibleClientRpc(_spawnedItem.GetComponent<NetworkObject>().NetworkObjectId);
        MakeItemVisibleServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void MakeItemVisibleClientRpc(ulong ItemID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ItemID, out var item);
        item.gameObject.SetActive(true);
    }
    
    [Rpc(SendTo.Server)]
    private void MakeItemVisibleServerRpc()
    {
        _spawnedItem.SetActive(true);
    }

    
    private void SpawnItem()
    {
        _spawnedItem = Instantiate(_Item.PickRandom(), transform.position, Quaternion.identity);
        if (_spawnedItem.TryGetComponent(out IGrabbable grabbable))
        { 
            grabbable.OnGrab.AddListener(StartRespawn); 
        } 
        _spawnedItem.GetComponent<NetworkObject>().Spawn();
    }
}

