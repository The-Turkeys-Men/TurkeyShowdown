using System.Collections.Generic;
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

    public void ShowScorePanel(ulong winnerId, Dictionary<ulong, int> playerScores)
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

    private void UpdateScoreDisplay(ulong winnerId, Dictionary<ulong, int> playerScores)
    {
        Debug.Log("Updating score display with player scores.");
        List<KeyValuePair<ulong, int>> sortedPlayers = new(playerScores);
        sortedPlayers.Sort((a, b) => b.Value.CompareTo(a.Value));

        if (firstPlayer == null)
        {
            Debug.LogError("FirstPlayer transform is null.");
            return;
        }

        if (sortedPlayers.Count > 0)
        {
            var firstPlayerData = sortedPlayers[0];
            Debug.Log($"First player: ID {firstPlayerData.Key}, Score {firstPlayerData.Value}");
            UpdateFirstPlayerDisplay(firstPlayer, firstPlayerData.Key, firstPlayerData.Value, 1);
        }

        if (scoreBoard == null)
        {
            Debug.LogError("ScoreBoard transform is null.");
            return;
        }

        // Mettre à jour directement les 7 entrées existantes
        for (int i = 1; i < sortedPlayers.Count && i <= 7; i++)
        {
            var playerData = sortedPlayers[i];

            if (i - 1 < scoreBoard.childCount)  // Vérifie si l'élément existe déjà
            {
                Transform playerScore = scoreBoard.GetChild(i - 1);
                Debug.Log($"Player {i}: ID {playerData.Key}, Score {playerData.Value}");
                UpdatePlayerDisplay(playerScore, playerData.Key, playerData.Value, i + 1);
            }
            else
            {
                Debug.LogWarning($"ScoreBoard does not have enough children. Expected at least {i}, but found {scoreBoard.childCount}.");
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
