using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class IconMapAttribution : NetworkBehaviour
{
    [SerializeField] private TeamComponent _teamComponent;
    [SerializeField] private Image _image;
    private NetworkObject _localPlayer;

    private void Start()
    {
        _image.gameObject.SetActive(true);
        _teamComponent.OnTeamChangedEvent += OnTeamChanged;
        UpdateLocalPlayer();
    }

    private void UpdateLocalPlayer()
    {
        ulong localPlayerId = NetworkManager.LocalClient.ClientId;
        _localPlayer = NetworkManager.SpawnManager.GetPlayerNetworkObject(localPlayerId);
        if (_localPlayer)
        {
            _localPlayer.GetComponent<TeamComponent>().OnTeamChangedEvent += OnTeamChanged;
            OnTeamChanged();
        }
    }

    private void Update()
    {
        if (_localPlayer)
        {
            return;
        }

        UpdateLocalPlayer();
    }

    private void OnTeamChanged()
    {
        if (!_localPlayer)
        {
            return;
        }

        if (_localPlayer.GetComponent<TeamComponent>().TeamID == _teamComponent.TeamID)
        {
            _image.color = Color.blue;
        }
        else
        {
            _image.color = Color.red;
        }
    }
}
