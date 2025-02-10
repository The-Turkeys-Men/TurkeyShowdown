using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Debugger;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public Dictionary<ulong, PlayerJSON> playerData = new();
    
    public static PlayerDataManager Datainstance;
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;
    
    private void Awake()
    {
        if (Datainstance == null)
        {
            Datainstance = this;
            DontDestroyOnLoad(this);
            //NetworkManager.Singleton.OnClientConnectedCallback += RefreshAllPlayers;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RefreshAllPlayers()
    {
        var players = NetworkManager.Singleton.ConnectedClients.Keys;
        foreach (var player in players)
        {
            NetworkManager.ConnectedClients[player].PlayerObject.GetComponent<ColorChanger>().ChangeColor(playerData[player].color);
        }
    }
    
    public async Task ReceivingJSON(ulong clientID)
    {
        Task<PlayerJSON> playerJson = _receivingJSON.FetchJSONValue();
        await playerJson;
        playerData.Add(clientID, playerJson.Result);
        RefreshAllPlayersServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void RefreshAllPlayersServerRpc()
    {
        RefreshAllPlayersClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RefreshAllPlayersClientRpc()
    {
        RefreshAllPlayers();
    }
    
    public void sendJSON()
    {
        StartCoroutine(_jsonSender.SendJsonToServer(JsonUtility.ToJson(playerData)));
    }
}
