using AYellowpaper.SerializedCollections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScorePanelManager : NetworkBehaviour
{
    public static ScorePanelManager Instance { get; private set; }

    [SerializeField] private GameObject scorePanel;
    [SerializeField] private Transform firstPlayer; // Référence au panneau du premier joueur
    [SerializeField] private Transform scoreBoard; // Référence au panneau des autres joueurs
    [SerializeField] private Image firstPlayerSkin; // Référence à l'image du premier joueur
    [SerializeField] private SerializedDictionary<string, Sprite> _sprites = new(); // Dictionnaire de sprites

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("ScorePanelManager instance created.");
        }
        else
        {
            Debug.LogWarning("Duplicate ScorePanelManager instance detected. Destroying the new one.");
            DestroyImmediate(this);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ShowScorePanelClientRpc(ulong winnerId, PlayerScore[] playerScores)
    {
        Debug.Log($"[ScorePanelManager] ShowScorePanelClientRpc called with {playerScores.Length} players.");
        ShowScorePanel(winnerId, playerScores);
    }

    public void ShowScorePanel(ulong winnerId, PlayerScore[] playerScores)
    {
        if (scorePanel == null)
        {
            Debug.LogError("[ScorePanelManager] Score panel is null. Please assign it in the editor.");
            return;
        }

        Debug.Log("[ScorePanelManager] Activating score panel.");
        scorePanel.SetActive(true);
        UpdateScoreDisplay(winnerId, playerScores);
    }

    private void UpdateScoreDisplay(ulong winnerId, PlayerScore[] playerScores)
    {
        Debug.Log("[ScorePanelManager] Updating score display.");
        if (playerScores == null || playerScores.Length == 0)
        {
            Debug.LogWarning("[ScorePanelManager] Player scores array is null or empty.");
            return;
        }

        PlayerScore[] sortedPlayers = playerScores.OrderByDescending(ps => ps.Score).ToArray();
        Debug.Log($"[ScorePanelManager] Number of players: {sortedPlayers.Length}");

        if (firstPlayer == null)
        {
            Debug.LogError("[ScorePanelManager] FirstPlayer transform is null.");
            return;
        }

        if (sortedPlayers.Length > 0)
        {
            var firstPlayerData = sortedPlayers[0];
            Debug.Log($"[ScorePanelManager] First player: ID {firstPlayerData.PlayerId}, Score {firstPlayerData.Score}");
            UpdateFirstPlayerDisplay(firstPlayer, firstPlayerData.PlayerId, firstPlayerData.Score, 1);
        }

        if (scoreBoard == null)
        {
            Debug.LogError("[ScorePanelManager] ScoreBoard transform is null.");
            return;
        }

        // Mettre à jour les autres joueurs (sans image)
        for (int i = 1; i < sortedPlayers.Length && i <= 7; i++)
        {
            var playerData = sortedPlayers[i];

            if (i - 1 < scoreBoard.childCount)  // Vérifie si l'élément existe déjà
            {
                Transform playerScore = scoreBoard.GetChild(i - 1);
                Debug.Log($"[ScorePanelManager] Player {i}: ID {playerData.PlayerId}, Score {playerData.Score}");
                UpdatePlayerDisplay(playerScore, playerData.PlayerId, playerData.Score, i + 1);
            }
            else
            {
                Debug.LogWarning($"[ScorePanelManager] ScoreBoard does not have enough children. Expected at least {i}, but found {scoreBoard.childCount}.");
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
            Debug.LogWarning("[ScorePanelManager] Player display transform is null.");
            return;
        }

        playerScore.gameObject.SetActive(true);
        SetText(playerScore, "PosImage/PosText", rank.ToString());

        // Récupérer le pseudo du joueur à partir de PlayerDataManager
        string playerName = (PlayerDataManager.Datainstance?.GetPlayerData(playerId)?.pseudo).ToString() ?? "Player " + playerId;
        Debug.Log($"[ScorePanelManager] Updating player display for ID {playerId}: {playerName}");
        SetText(playerScore, "PlayerImage/PlayerNameText", playerName);

        SetText(playerScore, "ScoreImage/ScoreText", score.ToString());
    }

    private void UpdateFirstPlayerDisplay(Transform playerDisplay, ulong playerId, int score, int rank)
    {
        if (playerDisplay == null)
        {
            Debug.LogWarning("[ScorePanelManager] First player display transform is null.");
            return;
        }

        playerDisplay.gameObject.SetActive(true);
        SetText(playerDisplay, "PlayerPosImage/PosText", rank.ToString());

        // Récupérer le pseudo du joueur à partir de PlayerDataManager
        string playerName = (PlayerDataManager.Datainstance?.GetPlayerData(playerId).pseudo).ToString() ?? "Player " + playerId;
        Debug.Log($"[ScorePanelManager] Updating first player display for ID {playerId}: {playerName}");
        SetText(playerDisplay, "PlayerNameImage/NameText", playerName);

        SetText(playerDisplay, "PlayerScoreImage/ScoreText", score.ToString());

        // Mettre à jour l'image du premier joueur
        if (firstPlayerSkin != null)
        {
            string skinName = PlayerDataManager.Datainstance?.GetPlayerData(playerId)?.color.ToString();
            if (!string.IsNullOrEmpty(skinName) && _sprites.ContainsKey(skinName))
            {
                firstPlayerSkin.sprite = _sprites[skinName];
            }
            else
            {
                Debug.LogWarning($"[ScorePanelManager] Skin '{skinName}' not found in the sprites dictionary.");
            }
        }
        else
        {
            Debug.LogWarning("[ScorePanelManager] First player skin image is null.");
        }
    }

    private void SetText(Transform parent, string path, string value)
    {
        Transform target = parent.Find(path);
        if (target == null)
        {
            Debug.LogWarning($"[ScorePanelManager] Could not find {path} under {parent.name}");
            return;
        }

        if (target.TryGetComponent<TextMeshProUGUI>(out var textComponent))
        {
            textComponent.text = value;
        }
        else
        {
            Debug.LogWarning($"[ScorePanelManager] Missing TextMeshProUGUI component on {path}");
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