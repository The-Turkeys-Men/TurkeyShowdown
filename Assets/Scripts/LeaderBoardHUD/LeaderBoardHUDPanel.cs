using TMPro;
using Unity.Netcode;

public class LeaderBoardHUDPanel : NetworkBehaviour
{
    public TextMeshProUGUI FirstPlaceText;
    public TextMeshProUGUI CurrentPlaceText;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsOwner)
        {
            if (LeaderBoardHUDManager.Instance != null)
            {
                LeaderBoardHUDManager.Instance.SetPanel(this);
            }
        }
    }
}
