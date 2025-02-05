using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardHUDPanel : MonoBehaviour
{
    public List<TextMeshProUGUI> LeaderBoardTexts = new();
    public TextMeshProUGUI CurrentPlaceText;
    private void Start()
    {
        LeaderBoardHUDManager.Instance.SetPanel(this);
    }
}
