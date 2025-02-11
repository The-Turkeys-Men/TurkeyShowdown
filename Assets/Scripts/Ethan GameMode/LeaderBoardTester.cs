using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LeaderboardTester : MonoBehaviour
{
    [SerializeField] private DeathMatchManager deathMatchManager;
    [SerializeField] private int numberOfPlayers = 5;

    private void Start()
    {
        if (deathMatchManager == null)
        {
            Debug.LogError("DeathMatchManager is not assigned in the LeaderboardTester.");
            return;
        }

        // Écoute l'événement de démarrage du serveur
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Serveur détecté, lancement du test du leaderboard.");
            SimulatePlayersAndScores();
        }
        else
        {
            Debug.LogError("LeaderboardTester doit être exécuté sur le serveur !");
        }
    }

    private void SimulatePlayersAndScores()
    {
        // Crée une liste temporaire de scores
        List<PlayerScore> simulatedScores = new();

        for (ulong i = 0; i < (ulong)numberOfPlayers; i++)
        {
            int randomScore = Random.Range(0, 20);
            simulatedScores.Add(new PlayerScore { PlayerId = i, Score = randomScore });
        }

        // Applique les scores via une méthode RPC
        UpdateScoresOnServerRpc(simulatedScores.ToArray());

        // Debug pour voir les scores attribués
        foreach (var playerScore in simulatedScores)
        {
            Debug.Log($"Joueur {playerScore.PlayerId} : {playerScore.Score} points");
        }

        // Simule la fin de la partie en désignant un gagnant
        deathMatchManager.EndGame(GetWinnerId());
    }

    [Rpc(SendTo.Server)]
    private void UpdateScoresOnServerRpc(PlayerScore[] scores)
    {
        // Nettoie les scores existants
        deathMatchManager.PlayerScores.Value.Clear();

        // Ajoute les nouveaux scores
        foreach (var score in scores)
        {
            deathMatchManager.PlayerScores.Value.Add(score);
        }

        // Force la mise à jour du NetworkVariable
        deathMatchManager.PlayerScores.SetDirty(true);
    }

    private ulong GetWinnerId()
    {
        ulong winnerId = ulong.MaxValue;
        int highestScore = 0;

        foreach (var playerScore in deathMatchManager.PlayerScores.Value)
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