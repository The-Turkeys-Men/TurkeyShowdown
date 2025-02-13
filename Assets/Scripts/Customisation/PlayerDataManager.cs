using System.Collections;
using System.Threading.Tasks;
using Customisation;
using Network;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Datainstance;
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;

    public PlayerDataNetworkable GetPlayerData(ulong clientId)
    {
        NetworkObject playerObject = NetworkManager.ConnectedClients[clientId].PlayerObject;

        if (!playerObject)
        {
            return null;
        }

        PlayerDataHolder playerDataHolder = playerObject.GetComponent<PlayerDataHolder>();
        return playerDataHolder.GetPlayerData();
    }

    public void ApplyPlayerData(PlayerDataNetworkable playerData)
    {
        NetworkObject playerObject = NetworkManager.ConnectedClients[playerData.ClientId].PlayerObject;

        if (!playerObject)
        {
            return;
        }

        PlayerDataHolder playerDataHolder = playerObject.GetComponent<PlayerDataHolder>();
        playerDataHolder.SetPseudoServerRpc(playerData.pseudo.ToString());
        playerDataHolder.SetColorServerRpc(playerData.color.ToString());
        
        RefreshAllPlayersServerRpc();
    }
    
    private void Awake()
    {
        if (Datainstance == null)
        {
            Datainstance = this;
            DontDestroyOnLoad(this);
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
            var playerObject = NetworkManager.ConnectedClients[player].PlayerObject;
            if (playerObject)
            {
                playerObject.GetComponent<ColorChanger>().ChangeSprite(GetPlayerData(player).color.ToString());
            }
        }
        
        LeaderBoardHUDManager.Instance.UpdateLeaderboardUI();
    }
    
    public IEnumerator WaitAndRefreshAllPlayers()
    {
        yield return new WaitForSeconds(2);
        RefreshAllPlayers();
    }
    
    public async Task ReceivingJSON(ulong clientID)
    {
        Task<PlayerJSON> playerJson = _receivingJSON.FetchJSONValue();
        await playerJson;
        
        PlayerDataNetworkable playerData = new()
        {
            ClientId = clientID,
            id = playerJson.Result.id,
            pseudo = playerJson.Result.pseudo,
            highScore = playerJson.Result.highScore,
            nbrVictory = playerJson.Result.nbrVictory,
            nbrDefeat = playerJson.Result.nbrDefeat,
            color = playerJson.Result.color,
        };

        ApplyPlayerData(playerData);
        
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void AddingPlayerJsonDataServerRpc(ulong clientID, PlayerDataNetworkable newData)
    {
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
        StartCoroutine(WaitAndRefreshAllPlayers());
    }
}
