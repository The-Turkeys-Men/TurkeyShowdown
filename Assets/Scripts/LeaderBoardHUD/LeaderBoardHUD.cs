using System;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class LeaderBoardHUD : NetworkBehaviour
{
    public TextMeshProUGUI FirstPlaceText;
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

    private readonly NetworkList<PlayerScore> _playerScores = new NetworkList<PlayerScore>();
    private int _playerCounter = 0;
    private const int POINTS_TO_ADD = 100;
    private const int POINTS_TO_REMOVE = 50;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        _playerScores.OnListChanged += OnScoresChanged;
        UpdateLeaderboardUI();
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            _playerCounter++; 
            string playerName = $"Player_{_playerCounter}";
            _playerScores.Add(new PlayerScore { ClientId = clientId, PlayerName = playerName, Score = 0 });
            Debug.Log($"[Server] New player connected: {playerName} ({clientId})");
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("[Server] Adding points to the first player");
                IncrementScoreForFirstPlayer();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("[Server] Removing points from the first player");
                DecrementScoreForFirstPlayer();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("[Server] Resetting leaderboard");
                ResetLeaderboard();
            }
        }
    }

    private void OnScoresChanged(NetworkListEvent<PlayerScore> changeEvent)
    {
        UpdateLeaderboardUI();
    }

    private void UpdateLeaderboardUI()
    {
        PlayerScore[] sortedScores = new PlayerScore[_playerScores.Count];
        for (int i = 0; i < _playerScores.Count; i++)
            sortedScores[i] = _playerScores[i];

        Array.Sort(sortedScores, (a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < sortedScores.Length; i++)
        {
            if (sortedScores[i].ClientId == NetworkManager.Singleton.LocalClientId)
            {
                CurrentPlaceText.text = $"#{i + 1} {sortedScores[i].PlayerName} - {sortedScores[i].Score}";
                break;
            }
        }

        if (sortedScores.Length > 0)
        {
            FirstPlaceText.text = $"#1 {sortedScores[0].PlayerName} - {sortedScores[0].Score}";
        }
    }

    private void IncrementScoreForFirstPlayer()
    {
        if (_playerScores.Count > 0)
        {
            PlayerScore updatedScore = _playerScores[0];
            updatedScore.Score += POINTS_TO_ADD;
            _playerScores[0] = updatedScore;
            Debug.Log($"[Server] {updatedScore.PlayerName} gains {POINTS_TO_ADD} points, total: {updatedScore.Score}");
        }
    }

    private void DecrementScoreForFirstPlayer()
    {
        if (_playerScores.Count > 0)
        {
            PlayerScore updatedScore = _playerScores[0];
            updatedScore.Score = Math.Max(0, updatedScore.Score - POINTS_TO_REMOVE);
            _playerScores[0] = updatedScore;
            Debug.Log($"[Server] {updatedScore.PlayerName} loses {POINTS_TO_REMOVE} points, total: {updatedScore.Score}");
        }
    }

    private void ResetLeaderboard()
    {
        _playerScores.Clear();
        _playerCounter = 0;
        Debug.Log("[Server] Leaderboard reset.");
        
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            _playerCounter++;
            string playerName = $"Player_{_playerCounter}";
            _playerScores.Add(new PlayerScore { ClientId = client.ClientId, PlayerName = playerName, Score = 0 });
        }
    }
}
