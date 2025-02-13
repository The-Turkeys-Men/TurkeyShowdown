using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerHUD : MonoBehaviour
{
    public static bool hasAlreadyStarted = false;
    
    private void Start()
    {
        if (hasAlreadyStarted && !Application.isEditor)
        {
            gameObject.SetActive(false);
            return;
        }
        
        hasAlreadyStarted = true;
        
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
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        gameObject.SetActive(false);
    }
    
    public void OnStartHost()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
        NetworkManager.Singleton.SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
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
