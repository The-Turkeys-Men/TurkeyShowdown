using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public PlayerJSON playerData;
    public NetworkVariable<Dictionary<ulong, string>> ColorData = new();
    
    public static PlayerDataManager Datainstance;
    
    public receivingJSON _receivingJSON;
    public JSONSender _jsonSender;
    
    private void Awake()
    {
        if (Datainstance == null)
        {
            Datainstance = this;
            DontDestroyOnLoad(this);
            NetworkManager.Singleton.OnClientConnectedCallback += RefreshAllPlayers;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RefreshAllPlayers(ulong _clientID)
    {
        var Players = NetworkManager.Singleton.ConnectedClients.Keys;
        foreach (var Player in Players)
        {
            NetworkManager.ConnectedClients[Player].PlayerObject.GetComponent<ColorChanger>().ChangeColor(ColorData.Value[Player]);
        }
    }
    
    public void AddColorData(ulong playerID, string color)
    {
        ColorData.Value.Add(playerID, color);
    }
    
    public async void receivingJSON()
    {
        Task<PlayerJSON> playerJson = _receivingJSON.FetchJSONValue();
        await playerJson;
        playerData = playerJson.Result;
    }

    public void sendJSON()
    {
        StartCoroutine(_jsonSender.SendJsonToServer(JsonUtility.ToJson(playerData)));
    }
}
