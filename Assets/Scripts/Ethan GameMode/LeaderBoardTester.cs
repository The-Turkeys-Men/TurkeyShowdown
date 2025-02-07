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
        // Nettoie les scores existants
        deathMatchManager.PlayerScores.Value.Clear();

        // Crée une liste temporaire de scores
        Dictionary<ulong, int> simulatedScores = new();

        for (ulong i = 0; i < (ulong)numberOfPlayers; i++)
        {
            int randomScore = Random.Range(0, 20);
            simulatedScores[i] = randomScore;
        }

        // Applique les scores et force la mise à jour du NetworkVariable
        deathMatchManager.PlayerScores.Value = simulatedScores;
        deathMatchManager.PlayerScores.SetDirty(true);

        // Debug pour voir les scores attribués
        foreach (var player in deathMatchManager.PlayerScores.Value)
        {
            Debug.Log($"Joueur {player.Key} : {player.Value} points");
        }

        // Simule la fin de la partie en désignant un gagnant
        deathMatchManager.EndGame(GetWinnerId());
    }

    private ulong GetWinnerId()
    {
        ulong winnerId = ulong.MaxValue;
        int highestScore = 0;

        foreach (var player in deathMatchManager.PlayerScores.Value)
        {
            if (player.Value > highestScore)
            {
                highestScore = player.Value;
                winnerId = player.Key;
            }
        }

        return winnerId;
    }
}
