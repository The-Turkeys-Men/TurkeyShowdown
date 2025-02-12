using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MapVote
{
    public class MapVoteObject : MonoBehaviour
    {
        public int MapIndex;

        [SerializeField] private TextMeshProUGUI _mapName;
        [SerializeField] private Image _mapImage;
        [SerializeField] private Button _voteButton;
        [SerializeField] private TextMeshProUGUI _voteButtonText;
        
        public void SetUp(int mapIndex)
        {
            MapIndex = mapIndex;
            MapVoteManager mapVoteManager = MapVoteManager.Instance;
            MapData mapData = mapVoteManager.MapDatas[mapIndex];
            _mapName.text = mapData.MapName;
            _mapImage.sprite = mapData.MapPreview;
            
            mapVoteManager.MapVotes.OnValueChanged += (value, newValue) => UpdateVoteNumber(newValue);
            
        }

        private void Start()
        {
            UpdateVoteNumber(MapVoteManager.Instance.MapVotes.Value);
        }

        public void OnVote()
        {
            var mapVoteManager = MapVoteManager.Instance;
            mapVoteManager.VoteForMapServerRpc(MapIndex);
            mapVoteManager.DisableVoteButtons();
        }
        
        public void DisableVoteButton()
        {
            GetComponentInChildren<Button>().interactable = false;
        }

        private void UpdateVoteNumber(Dictionary<int, int> current)
        {
            int voteNumber = current[MapIndex];
            string textSuffix = voteNumber > 1 ? "s" : "";
            _voteButtonText.text = voteNumber + $" Vote{textSuffix}";
        }
    }
}