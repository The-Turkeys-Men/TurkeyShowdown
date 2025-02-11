using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Customisation;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Datainstance;
    public NetworkVariable<List<PlayerDataNetworkable>> playerDatas = new();

    public NetworkVariable<List<int>> playerScores = new();
    
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
        bool replacedData = false;
        for (int i = 0; i < playerDatas.Value.Count; i++)
        {
            if (playerDatas.Value[i].ClientId == clientID)
            {
                playerDatas.Value[i] = newData;
                replacedData = true;
                Debug.Log("Replaced existing player data for clientID: " + clientID);
                break;
            }
        }
    
        if (!replacedData)
        {
            playerDatas.Value.Add(newData);
            Debug.Log("Added new player data for clientID: " + clientID);
        }
        
        Debug.Log("Received JSON with pseudo: " + newData.pseudo);
        RefreshAllPlayersClientRpc();
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
