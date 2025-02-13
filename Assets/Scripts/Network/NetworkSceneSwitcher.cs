using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Network
{
    public class NetworkSceneSwitcher : NetworkBehaviour
    {
        public static NetworkSceneSwitcher Instance;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        public void SwitchScene(Scene currentScene, string sceneName)
        {
            if (IsServer)
            {
                DisconnectAndReconnectEveryoneRpc();
                NetworkManager.Singleton.SceneManager.UnloadScene(currentScene);
                StartCoroutine(SwitchSceneCoroutine(sceneName));
            }
        }

        public IEnumerator SwitchSceneCoroutine(string sceneName)
        {
            
            yield return new WaitForSeconds(1);
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            yield return new WaitForSeconds(1);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            yield return null;
        }

        [Rpc(SendTo.Everyone, RequireOwnership = false)]
        public void DisconnectAndReconnectEveryoneRpc()
        {
            if (IsHost || IsServer)
            {
                return;
            }
            DisconnectAndReconnect();
        }

        public void DisconnectAndReconnect()
        {
            NetworkManager.Shutdown();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            StartCoroutine(Reconnect());
        }

        private IEnumerator Reconnect()
        {
            yield return new WaitForSeconds(5);
            NetworkManager.Singleton.StartClient();
        }
    }
}