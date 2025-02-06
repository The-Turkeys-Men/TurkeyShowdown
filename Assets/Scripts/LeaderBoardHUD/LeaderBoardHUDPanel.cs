using System;
using System.Collections.Generic;
using Debugger;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardHUDPanel : NetworkBehaviour
{
    public List<TextMeshProUGUI> LeaderBoardTexts = new();
    public TextMeshProUGUI CurrentPlaceText;
    
    private void Start()
    {
        if (IsOwner)
        {
            LeaderBoardHUDManager.Instance.SetPanel(this);
        }
    }
}
