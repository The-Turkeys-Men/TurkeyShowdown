using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customisation;
using Debugger;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDataManager : NetworkBehaviour
{
    public NetworkVariable<List<PlayerDataNetworkable>> playerDatas = new();
    
    public static PlayerDataManager Datainstance;
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;

    public PlayerDataNetworkable GetPlayerData(ulong clientId)
    {
        foreach (var playerData in playerDatas.Value.ToList())
        {
            if (playerData.ClientId == clientId)
            {
                return playerData;
            }
        }

        return null;
    }
    
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
        Debug.Log("Refreshing all players");
        var players = NetworkManager.Singleton.ConnectedClients.Keys;
        foreach (var player in players)
        {
            Debug.Log("Refreshing player with clientId: " + player);
            NetworkManager.ConnectedClients[player].PlayerObject.GetComponent<ColorChanger>().ChangeColor(GetPlayerData(player).color.ToString());
        }
        Debug.Log("Finished refreshing all players");
    }
    
    public async Task ReceivingJSON(ulong clientID)
    {
        Debug.Log("Starting ReceivingJSON for clientID: " + clientID);
        Task<PlayerJSON> playerJson = _receivingJSON.FetchJSONValue();
        await playerJson;
        Debug.Log("Received JSON data for clientID: " + clientID);
    
        PlayerDataNetworkable playerDataNetworkable = new()
        {
            ClientId = clientID,
            id = playerJson.Result.Id,
            pseudo = playerJson.Result.Pseudo,
            highScore = playerJson.Result.HighScore,
            nbrVictory = playerJson.Result.NbrVictory,
            nbrDefeat = playerJson.Result.NbrDefeat,
            color = playerJson.Result.Color,
            scoreTable = playerJson.Result.ScoreTable.ToList(),
            skins = playerJson.Result.Skins.ToList()
        };
        Debug.Log("Created PlayerDataNetworkable for clientID: " + clientID);
    
        bool replacedData = false;
        for (int i = 0; i < playerDatas.Value.Count; i++)
        {
            if (playerDatas.Value[i].ClientId == clientID)
            {
                playerDatas.Value[i] = playerDataNetworkable;
                replacedData = true;
                Debug.Log("Replaced existing player data for clientID: " + clientID);
                break;
            }
        }
    
        if (!replacedData)
        {
            playerDatas.Value.Add(playerDataNetworkable);
            Debug.Log("Added new player data for clientID: " + clientID);
        }
        
        Debug.Log("Received JSON with pseudo: " + playerJson.Result.Pseudo);
        RefreshAllPlayersServerRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RefreshAllPlayersServerRpc()
    {
        RefreshAllPlayersClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void RefreshAllPlayersClientRpc()
    {
        RefreshAllPlayers();
    }
    
    public void sendJSON()
    {
        StartCoroutine(_jsonSender.SendJsonToServer(JsonUtility.ToJson(playerDatas)));
    }
}
