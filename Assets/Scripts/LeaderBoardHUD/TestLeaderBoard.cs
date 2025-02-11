using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestLeaderBoard : NetworkBehaviour
{
    [SerializeField] private float updateInterval = 1f; // Intervalle de mise à jour en secondes
    [SerializeField] private int maxScoreIncrement = 10; // Score maximum à ajouter à chaque mise à jour

    private List<PlayerScore> playerScores = new List<PlayerScore>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("Server is ready. Initializing player scores and starting score updates.");
            InitializePlayerScores();
            StartCoroutine(UpdateScoresRoutine());
        }
    }

    private void InitializePlayerScores()
    {
        // Simuler quelques joueurs avec des scores initiaux
        playerScores.Add(new PlayerScore { PlayerId = 1, PlayerName = "Player1", Score = 0 });
        playerScores.Add(new PlayerScore { PlayerId = 2, PlayerName = "Player2", Score = 0 });
        playerScores.Add(new PlayerScore { PlayerId = 3, PlayerName = "Player3", Score = 0 });

        // Mettre à jour la liste des scores dans le DeathMatchManager
        DeathMatchManager.GetInstance().PlayerScores.Value = playerScores;
        Debug.Log("Player scores initialized.");
    }

    private IEnumerator UpdateScoresRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            // Ajouter un score aléatoire à chaque joueur
            for (int i = 0; i < playerScores.Count; i++)
            {
                PlayerScore updatedScore = playerScores[i];
                int randomIncrement = Random.Range(0, maxScoreIncrement + 1);
                updatedScore.Score += randomIncrement;
                playerScores[i] = updatedScore; // Mettre à jour la structure dans la liste
            }

            // Mettre à jour la liste des scores dans le DeathMatchManager
            DeathMatchManager.GetInstance().PlayerScores.Value = playerScores;
            Debug.Log("Player scores updated.");

            // Forcer la mise à jour de l'UI via le LeaderBoardHUDManager
            if (LeaderBoardHUDManager.Instance != null)
            {
                LeaderBoardHUDManager.Instance.UpdateLeaderboardUI();
            }
        }
    }
}