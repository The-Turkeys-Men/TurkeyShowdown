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
    
    public NetworkDictionary<ulong, int> PlayerIds = new();
    public NetworkDictionary<ulong, FixedString32Bytes> PlayerPseudos = new();
    public NetworkDictionary<ulong, int> PlayerHighScores = new();
    public NetworkDictionary<ulong, int> PlayerNbrVictory = new();
    public NetworkDictionary<ulong, int> PlayerNbrDefeat = new();
    public NetworkDictionary<ulong, FixedString32Bytes> PlayerColors = new();
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;

    public PlayerDataNetworkable GetPlayerData(ulong clientId)
    {
        PlayerDataNetworkable playerData = new()
        {
            id = PlayerIds[clientId],
            pseudo = PlayerPseudos[clientId],
            highScore = PlayerHighScores[clientId],
            nbrVictory = PlayerNbrVictory[clientId],
            nbrDefeat = PlayerNbrDefeat[clientId],
            color = PlayerColors[clientId],
        };

        return playerData;
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
            scoreTable = playerJson.Result.scoreTable.ToList(),
            skins = playerJson.Result.skins.ToList()
        };
        Debug.Log("Created PlayerDataNetworkable for clientID: " + clientID);
    
        AddingPlayerJsonDataServerRpc(clientID, playerDataNetworkable);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void AddingPlayerJsonDataServerRpc(ulong clientID, PlayerDataNetworkable newData)
    {
        PlayerIds.Add(clientID, newData.id);
        PlayerPseudos.Add(clientID, newData.pseudo);
        PlayerColors.Add(clientID, newData.color);
        PlayerHighScores.Add(clientID, newData.highScore);
        PlayerNbrVictory.Add(clientID, newData.nbrVictory);
        PlayerNbrDefeat.Add(clientID, newData.nbrDefeat);
        
        PlayerIds.SetDirty(true);
        PlayerPseudos.SetDirty(true);
        PlayerHighScores.SetDirty(true);
        PlayerNbrVictory.SetDirty(true);
        PlayerNbrDefeat.SetDirty(true);
        PlayerColors.SetDirty(true);
        
        Debug.Log("Added player data to dictionaries for clientID: " + clientID);

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
