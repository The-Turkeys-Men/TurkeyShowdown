using System;
using System.Collections.Generic;
using Extensions;
using Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapVote
{
    public class MapVoteManager : NetworkBehaviour
    {
        public static MapVoteManager Instance;

        public List<MapData> MapDatas = new();

        public NetworkDictionary<int, int> MapVotes = new();
        
        private bool _isVoting;
        [SerializeField] private float _voteTime = 30f;

        [SerializeField] private Transform _mapVotePanel;
        [SerializeField] private Transform _mapVoteUIParent;
        [SerializeField] private MapVoteObject _mapVoteObjectPrefab;
        
        [SerializeField] private TextMeshProUGUI _timerText;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            _mapVotePanel.gameObject.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsServer)
            {
                SetUpMapVoteServer();
            }
            
            SetUpMapVoteUI();
        }

        private void SetUpMapVoteServer()
        {
            for (int i = 0; i < MapDatas.Count; i++)
            {
                MapVotes.Add(i, 0);
            }
        }
        
        private void SetUpMapVoteUI()
        {
            for (int i = 0; i < MapDatas.Count; i++)
            {
                var mapVoteObject = Instantiate(_mapVoteObjectPrefab, _mapVoteUIParent);
                mapVoteObject.SetUp(i);
            }
        }

        public void DisableVoteButtons()
        {
            foreach (Transform mapVoteObject in _mapVoteUIParent)
            {
                mapVoteObject.GetComponent<MapVoteObject>().DisableVoteButton();
            }
        }

        private void Update()
        {
            if (!IsServer || !_isVoting)
            {
                return;
            }
            
            _voteTime -= Time.deltaTime;
            UpdateTimerRpc((int)_voteTime);
            if (_voteTime <= 0)
            {
                EndMapVote();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateTimerRpc(int time)
        {
            _timerText.text = $"Temps restant: {time}";
        }

        public void StartMapVote()
        {
            Debug.Log("starting map vote");
            NetworkManager.Singleton.SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
            _isVoting = true;
        }

        [Rpc(SendTo.Server)]
        public void VoteForMapServerRpc(int mapIndex)
        {
            MapVotes[mapIndex]++;
            Debug.Log("Received vote for map " + mapIndex + " with now " + MapVotes[mapIndex] + " votes.");
        }

        private void EndMapVote()
        {
            _isVoting = false;
            
            int highestVotes = int.MinValue;
            List<int> chosenMaps = new();
            foreach (var mapIndex in MapVotes.Keys)
            {
                int currentMapVotes = MapVotes[mapIndex];
                if (currentMapVotes > highestVotes)
                {
                    chosenMaps.Clear();
                    chosenMaps.Add(mapIndex);
                    highestVotes = currentMapVotes;
                }
                else if (currentMapVotes == highestVotes)
                {
                    chosenMaps.Add(mapIndex);
                }
            }

            int nextMapIndex = chosenMaps.PickRandom();
            
            /*var networkObjects = FindObjectsOfType<NetworkObject>();
            foreach (var networkObject in networkObjects)
            {
                if (networkObject.gameObject.scene.name == "DontDestroyOnLoad")
                {
                    continue;
                }
                networkObject.Despawn(true);
            }*/
            
            NetworkSceneSwitcher.Instance.SwitchScene(gameObject.scene, MapDatas[nextMapIndex].SceneName);
        }
    }
}