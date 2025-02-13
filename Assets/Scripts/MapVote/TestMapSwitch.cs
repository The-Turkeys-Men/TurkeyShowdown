using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMapSwitch : NetworkBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && Application.isFocused)
        {
            TryLoadSceneServerRpc("MapVote2");
        }
    }
    
    [Rpc(SendTo.Server)]
    private void TryLoadSceneServerRpc(string sceneName)
    {
        NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
