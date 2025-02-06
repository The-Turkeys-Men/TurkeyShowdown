using System;
using System.Collections.Generic;
using System.Linq;
using Debugger;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class LeaderBoardHUDManager : NetworkBehaviour
{
    public static LeaderBoardHUDManager Instance;
    
    public List<TextMeshProUGUI> LeaderBoardTexts = new();
    public TextMeshProUGUI CurrentPlaceText;

    private struct PlayerScore : INetworkSerializable, IEquatable<PlayerScore>
    {
        public ulong ClientId;
        public FixedString64Bytes PlayerName;
        public int Score;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Score);
        }

        public bool Equals(PlayerScore other) => ClientId == other.ClientId && Score == other.Score;
        public override bool Equals(object obj) => obj is PlayerScore other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(ClientId, Score);
    }

    public void SetPanel(LeaderBoardHUDPanel panel)
    {
        LeaderBoardTexts = panel.LeaderBoardTexts;
        CurrentPlaceText = panel.CurrentPlaceText;
        Invoke(nameof(UpdateLeaderboardUI), 0.25f);
    }
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        DeathMatchManager.Instance.PlayerScores.OnValueChanged += OnScoresChanged;
        //UpdateLeaderboardUI();
    }
    
    private void OnScoresChanged(Dictionary<ulong, int> previousvalue, Dictionary<ulong, int> newvalue)
    {
        UpdateLeaderboardUI();
    }

    private void UpdateLeaderboardUI()
    {
        if (!CurrentPlaceText || LeaderBoardTexts.Count == 0)
        {
            return;
        }

        Dictionary<ulong, int> gmPlayerScores = DeathMatchManager.Instance.PlayerScores.Value;

        PlayerScore[] sortedScores = new PlayerScore[gmPlayerScores.Count];
        for (int i = 0; i < gmPlayerScores.Count; i++)
        {
            var playerScore = gmPlayerScores.ElementAt(i);
            PlayerScore updatedScore = new()
            {
                ClientId = playerScore.Key,
                PlayerName = $"Player_{i + 1}",
                Score = playerScore.Value,
            };
            DebuggerConsole.Instance.Log($"Player {updatedScore.PlayerName} has {updatedScore.Score} points.");
            sortedScores[i] = updatedScore;
        }

        Array.Sort(sortedScores, (a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < sortedScores.Length; i++)
        {
            if (sortedScores[i].ClientId == NetworkManager.Singleton.LocalClientId)
            {
                CurrentPlaceText.text = $"#{i + 1} {sortedScores[i].PlayerName} - {sortedScores[i].Score}";
                break;
            }
        }

        
        for (int i = 0; i < sortedScores.Length; i++)
        {
            if (i >= LeaderBoardTexts.Count)
            {
                break;
            }
            LeaderBoardTexts[i].text = $"#{i + 1} {sortedScores[i].PlayerName} - {sortedScores[i].Score}";
        }
    }
}
