using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoardHUDPanel : NetworkBehaviour
{
    public TextMeshProUGUI FirstPlaceText;
    public TextMeshProUGUI CurrentPlaceText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            Debug.Log("[LeaderBoardHUDPanel] Client connected. Assigning panel to LeaderBoardHUDManager.");
            
            if (LeaderBoardHUDManager.Instance != null)
            {
                LeaderBoardHUDManager.Instance.SetPanel(this);
                Debug.Log("[LeaderBoardHUDPanel] Panel successfully assigned.");
            }
            else
            {
                Debug.LogError("[LeaderBoardHUDPanel] Error: LeaderBoardHUDManager.Instance is NULL!");
            }
        }
    }
}
