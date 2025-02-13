using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMenu : NetworkBehaviour
{
    public void OnSwitchScene()
    {
        OnSwitchSceneServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void OnSwitchSceneServerRpc()
    {
        NetworkManager.SceneManager.LoadScene("Ferme", LoadSceneMode.Additive);
    }
}
