using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customisation;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Datainstance;
    
    public NetworkVariable<List<PlayerDataNetworkable>> PlayerDataList = new(new List<PlayerDataNetworkable>());
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;

    public PlayerDataNetworkable GetPlayerData(ulong clientId)
    {
        foreach (PlayerDataNetworkable playerData in PlayerDataList.Value.ToList())
        {
            if (clientId == playerData.ClientId)
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
            var playerObject = NetworkManager.ConnectedClients[player].PlayerObject;
            if (playerObject)
            {
                playerObject.GetComponent<ColorChanger>().ChangeColor(GetPlayerData(player).color.ToString());
                Debug.Log("Refreshing player with clientId: " + player);
            }
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
            id = playerJson.Result.id,
            pseudo = playerJson.Result.pseudo,
            highScore = playerJson.Result.highScore,
            nbrVictory = playerJson.Result.nbrVictory,
            nbrDefeat = playerJson.Result.nbrDefeat,
            color = playerJson.Result.color,
        };
        Debug.Log("Created PlayerDataNetworkable for clientID: " + clientID);

        Debug.Log("sending data");
        AddingPlayerJsonDataServerRpc(clientID, playerDataNetworkable);
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void AddingPlayerJsonDataServerRpc(ulong clientID, PlayerDataNetworkable newData)
    {
        PlayerDataList.Value.Add(newData);
        Debug.Log("AIOJEAFIOA Added player data to list for clientID: " + clientID);

        RefreshAllPlayersClientRpc();
        
        return;
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
}
