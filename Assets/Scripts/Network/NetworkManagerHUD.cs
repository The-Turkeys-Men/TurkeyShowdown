using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerHUD : MonoBehaviour
{
    private void Start()
    {
        if (Application.isBatchMode)
        {
            OnStartServer();
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            OnStartClient();
        }
    }

    public void OnStartServer()
    {
        NetworkManager.Singleton.StartServer();
        gameObject.SetActive(false);
    }
    
    public void OnStartHost()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }
    
    public void OnStartClient()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
