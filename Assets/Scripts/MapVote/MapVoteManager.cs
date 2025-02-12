using System;
using System.Collections.Generic;
using Extensions;
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
        
        public NetworkVariable<Dictionary<int, int>> MapVotes = new(new Dictionary<int, int>());
        
        private bool _isVoting;
        [SerializeField] private float _voteTime = 30f;
        
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
            
            if (IsServer)
            {
                SetUpMapVoteServer();
            }
            
            SetUpMapVoteUI();
            gameObject.SetActive(false);
        }

        private void SetUpMapVoteServer()
        {
            for (int i = 0; i < MapDatas.Count; i++)
            {
                MapVotes.Value.Add(i, 0);
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
            if (!_isVoting)
            {
                return;
            }
            
            _voteTime -= Time.deltaTime;
            _timerText.text = $"Temps restant: {(int)_voteTime}";
            if (_voteTime <= 0)
            {
                EndMapVote();
            }
        }

        public void StartMapVote()
        {
            _isVoting = true;
        }

        [Rpc(SendTo.Server)]
        public void VoteForMapServerRpc(int mapIndex)
        {
            MapVotes.Value[mapIndex]++;
        }

        private void EndMapVote()
        {
            _isVoting = false;
            
            int highestVotes = int.MinValue;
            List<int> chosenMaps = new();
            foreach (var mapIndex in MapVotes.Value.Keys)
            {
                int currentMapVotes = MapVotes.Value[mapIndex];
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
            NetworkManager.Singleton.SceneManager.LoadScene(MapDatas[nextMapIndex].SceneName, LoadSceneMode.Single);
        }
    }
}