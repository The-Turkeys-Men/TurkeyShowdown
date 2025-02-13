using Unity.Netcode;
using UnityEngine;

public class NetworkSceneTransitioner : MonoBehaviour
{

    [ContextMenu("CACACCAAACACACA")]
    public void ActivateAll()
    {
        var all = FindObjectsByType<NetworkObject>(FindObjectsSortMode.None);
        foreach (NetworkObject networkObject in all)
        {
            networkObject.ActiveSceneSynchronization = true;
        }
    }
}
