using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class ScorePanelManager : NetworkBehaviour
{
    public static ScorePanelManager Instance { get; private set; }

    [SerializeField] private GameObject scorePanel;
    [SerializeField] private Transform firstPlayer;
    [SerializeField] private Transform scoreBoard;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShowScorePanelClientRpc(ulong winnerId, PlayerScore[] playerScores)
    {
        ShowScorePanel(winnerId, playerScores);
    }

    public void ShowScorePanel(ulong winnerId, PlayerScore[] playerScores)
    {
        if (scorePanel == null)
        {
            Debug.LogError("Score panel is null. Please assign it in the editor.");
            return;
        }

        Debug.Log("Score panel is not null, activating it.");
        scorePanel.SetActive(true);
        UpdateScoreDisplay(winnerId, playerScores);
    }

    private void UpdateScoreDisplay(ulong winnerId, PlayerScore[] playerScores)
    {
        Debug.Log("Updating score display with player scores.");
        PlayerScore[] sortedPlayers = playerScores.OrderByDescending(ps => ps.Score).ToArray();

        if (firstPlayer == null)
        {
            Debug.LogError("FirstPlayer transform is null.");
            return;
        }

        if (sortedPlayers.Length > 0)
        {
            var firstPlayerData = sortedPlayers[0];
            Debug.Log($"First player: ID {firstPlayerData.PlayerId}, Score {firstPlayerData.Score}");
            UpdateFirstPlayerDisplay(firstPlayer, firstPlayerData.PlayerId, firstPlayerData.Score, 1);
        }

        if (scoreBoard == null)
        {
            Debug.LogError("ScoreBoard transform is null.");
            return;
        }

        // Mettre à jour directement les 7 entrées existantes
        for (int i = 1; i < sortedPlayers.Length && i <= 7; i++)
        {
            var playerData = sortedPlayers[i];

            if (i - 1 < scoreBoard.childCount)  // Vérifie si l'élément existe déjà
            {
                Transform playerScore = scoreBoard.GetChild(i - 1);
                Debug.Log($"Player {i}: ID {playerData.PlayerId}, Score {playerData.Score}");
                UpdatePlayerDisplay(playerScore, playerData.PlayerId, playerData.Score, i + 1);
            }
            else
            {
                Debug.LogWarning($"ScoreBoard does not have enough children. Expected at least {i}, but found {scoreBoard.childCount}.");
            }
        }

        // Désactiver les entrées inutilisées
        for (int i = sortedPlayers.Length; i < 8; i++)  // 8 entrées au total (indices 0 à 7)
        {
            if (i - 1 < scoreBoard.childCount)
            {
                Transform playerScore = scoreBoard.GetChild(i - 1);
                playerScore.gameObject.SetActive(false);  // Désactiver l'entrée
            }
        }
    }

    private void UpdatePlayerDisplay(Transform playerScore, ulong playerId, int score, int rank)
    {
        if (playerScore == null)
        {
            Debug.LogWarning("Player display transform is null.");
            return;
        }

        playerScore.gameObject.SetActive(true);
        SetText(playerScore, "PosImage/PosText", rank.ToString());
        SetText(playerScore, "PlayerImage/PlayerNameText", $"Player {playerId}");
        SetText(playerScore, "ScoreImage/ScoreText", score.ToString());
    }

    private void UpdateFirstPlayerDisplay(Transform playerDisplay, ulong playerId, int score, int rank)
    {
        if (playerDisplay == null)
        {
            Debug.LogWarning("First player display transform is null.");
            return;
        }

        playerDisplay.gameObject.SetActive(true);
        SetText(playerDisplay, "PlayerPosImage/PosText", rank.ToString());
        SetText(playerDisplay, "PlayerNameImage/NameText", $"Player {playerId}");
        SetText(playerDisplay, "PlayerScoreImage/ScoreText", score.ToString());
    }

    private void SetText(Transform parent, string path, string value)
    {
        Transform target = parent.Find(path);
        if (target == null)
        {
            Debug.LogWarning($"Could not find {path} under {parent.name}");
            return;
        }

        if (target.TryGetComponent<TextMeshProUGUI>(out var textComponent))
        {
            textComponent.text = value;
        }
        else
        {
            Debug.LogWarning($"Missing TextMeshProUGUI component on {path}");
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void HideScorePanelClientRpc()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
    }
}