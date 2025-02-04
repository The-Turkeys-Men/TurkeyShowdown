using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerHUD : MonoBehaviour
{
    private void Start()
    {
        if (Application.isBatchMode)
        {
            NetworkManager.Singleton.StartServer();
        }
    }

    public void OnStartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    public void OnStartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    public void OnStartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
