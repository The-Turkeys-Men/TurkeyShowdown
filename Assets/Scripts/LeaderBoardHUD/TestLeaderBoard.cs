// using System.Collections.Generic;
// using UnityEngine;
//
// public class TestLeaderboard : MonoBehaviour
// {
//     private DeathMatchManager deathMatchManager;
//
//     private void Start()
//     {
//         // Récupérer le GameModeManager et vérifier si c'est un DeathMatchManager
//         if (GameModeManager.GetInstance() is DeathMatchManager dmManager)
//         {
//             deathMatchManager = dmManager;
//         }
//         else
//         {
//             Debug.LogError("[TestLeaderboard] DeathMatchManager introuvable !");
//             return;
//         }
//
//         // Initialiser des scores fictifs
//         InitTestScores();
//     }
//
//     private void InitTestScores()
//     {
//         List<PlayerScore> testScores = new List<PlayerScore>
//         {
//             new PlayerScore { PlayerId = 1, PlayerName = "Joueur 1", Score = 10 },
//             new PlayerScore { PlayerId = 2, PlayerName = "Joueur 2", Score = 15 },
//             new PlayerScore { PlayerId = 3, PlayerName = "Joueur 3", Score = 20 }
//         };
//
//         // Simuler l'assignation des scores
//         deathMatchManager.PlayerScores.Value = testScores;
//
//         Debug.Log("[TestLeaderboard] Scores initiaux ajoutés !");
//         LeaderBoardHUDManager.Instance.ForceUpdateLeaderboard();
//     }
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             // Récupérer la liste actuelle et la copier
//             List<PlayerScore> updatedScores = new List<PlayerScore>(deathMatchManager.PlayerScores.Value);
//
//             // Trouver le joueur 1 et lui ajouter des points
//             for (int i = 0; i < updatedScores.Count; i++)
//             {
//                 if (updatedScores[i].PlayerId == 1)
//                 {
//                     PlayerScore newScore = updatedScores[i];
//                     newScore.Score += 5;
//                     updatedScores[i] = newScore;
//                     break;
//                 }
//             }
//
//             // Mettre à jour le leaderboard
//             deathMatchManager.PlayerScores.Value = updatedScores;
//
//             Debug.Log("[TestLeaderboard] Joueur 1 a gagné +5 points !");
//             LeaderBoardHUDManager.Instance.ForceUpdateLeaderboard();
//         }
//     }
// }



// DANS Modifications dans LeaderBoardHUDManager.cs
// public void ForceUpdateLeaderboard()
// {
//     if (panel == null) return;
//
//     UpdateLeaderboardClientRpc(panel.FirstPlaceText.text, panel.CurrentPlaceText.text);
// }