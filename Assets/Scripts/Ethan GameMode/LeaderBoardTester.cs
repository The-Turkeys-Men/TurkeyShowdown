using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LeaderboardTester : MonoBehaviour
{
    [SerializeField] private DeathMatchManager _deathMatchManager;
    [SerializeField] private int _numberOfPlayers = 5;

    private void Start()
    {
        if (_deathMatchManager == null)
        {
            Debug.LogError("DeathMatchManager is not assigned in LeaderboardTester.");
            return;
        }

        // Listen to the server start event
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server detected, launching leaderboard test.");
            SimulatePlayersAndScores();
        }
        else
        {
            Debug.LogError("LeaderboardTester must be run on the server!");
        }
    }

    private void SimulatePlayersAndScores()
    {
        // Clear existing scores
        _deathMatchManager.PlayerScores.Value.Clear();

        // Create a temporary list of scores
        List<PlayerScore> simulatedScores = new();

        for (ulong i = 0; i < (ulong)_numberOfPlayers; i++)
        {
            int randomScore = Random.Range(0, 20);
            simulatedScores.Add(new PlayerScore { PlayerId = i, Score = randomScore });
        }

        // Apply scores and force NetworkVariable update
        _deathMatchManager.PlayerScores.Value = simulatedScores;
        _deathMatchManager.PlayerScores.SetDirty(true);

        // Debug to see assigned scores
        foreach (var playerScore in _deathMatchManager.PlayerScores.Value)
        {
            Debug.Log($"Player {playerScore.PlayerId}: {playerScore.Score} points");
        }

        // Simulate the end of the game by selecting a winner
        _deathMatchManager.EndGame(GetWinnerId());
    }

    private ulong GetWinnerId()
    {
        ulong winnerId = ulong.MaxValue;
        int highestScore = 0;

        foreach (var playerScore in _deathMatchManager.PlayerScores.Value)
        {
            if (playerScore.Score > highestScore)
            {
                highestScore = playerScore.Score;
                winnerId = playerScore.PlayerId;
            }
        }

        return winnerId;
    }
}
