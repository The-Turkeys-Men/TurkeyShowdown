using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _Item;
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
        yield return new WaitForSeconds(_respawnTime);
        if (!_spawnedItem || _reinstantiate)
        {
            SpawnItem();
        }
        MakeItemVisibleClientRpc(_spawnedItem.GetComponent<NetworkObject>().NetworkObjectId);
        MakeItemVisibleServerRpc();
    }

    [ClientRpc]
    private void MakeItemVisibleClientRpc(ulong ItemID)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ItemID, out var item);
        item.gameObject.SetActive(true);
    }
    
    [ServerRpc]
    private void MakeItemVisibleServerRpc()
    {
        _spawnedItem.SetActive(true);
    }

    
    private void SpawnItem()
    {
        _spawnedItem = Instantiate(_Item, transform.position, Quaternion.identity);
        if (_spawnedItem.TryGetComponent(out IGrabbable grabbable))
        { 
            grabbable.OnGrab.AddListener(StartRespawn); 
        } 
        _spawnedItem.GetComponent<NetworkObject>().Spawn();
    }
}

