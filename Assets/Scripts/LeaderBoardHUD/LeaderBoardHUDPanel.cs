using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardHUDPanel : NetworkBehaviour
{
    public List<TextMeshProUGUI> LeaderBoardTexts = new();
    public TextMeshProUGUI CurrentPlaceText;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            LeaderBoardHUDManager.Instance.SetPanel(this);
        }
    }
}
