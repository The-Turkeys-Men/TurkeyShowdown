using System;
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
        }

        public void SwitchScene(Scene currentScene, string sceneName)
        {
            if (IsServer)
            {
                NetworkManager.Singleton.SceneManager.UnloadScene(currentScene);
                StartCoroutine(SwitchSceneCoroutine(sceneName));
            }
        }

        public IEnumerator SwitchSceneCoroutine(string sceneName)
        {
            yield return new WaitForSeconds(3);
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            yield return null;
        }
    }
}