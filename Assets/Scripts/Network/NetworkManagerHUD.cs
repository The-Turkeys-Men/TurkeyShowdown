using System;
using System.Linq;
using System.Threading.Tasks;
using Network;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerHUD : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private RelayManager _relayManager;
    
    private void Start()
    {
        TryStartBatchMode();
    }

    private async Task TryStartBatchMode()
    {
        if (!Application.isBatchMode)
        {
            return;
        }

        _relayManager.CreateRelay(10);
    }

    public void OnStartServer()
    {
        _relayManager.CreateRelay(10);
    }

    public void OnStartHost()
    {
        Debug.Log("mdr");
    }
    
    public void OnStartClient()
    {
        print("entered code : " + _joinCodeInputField.text + "with length : " + _joinCodeInputField.text.Length);
        _relayManager.JoinRelay(_joinCodeInputField.text);
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
