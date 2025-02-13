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
            return;
        }
        
        scorePanel.SetActive(true);
        UpdateScoreDisplay(winnerId, playerScores);
    }

    private void UpdateScoreDisplay(ulong winnerId, PlayerScore[] playerScores)
    {
        if (playerScores == null || playerScores.Length == 0)
        {
            return;
        }

        PlayerScore[] sortedPlayers = playerScores.OrderByDescending(ps => ps.Score).ToArray();

        if (firstPlayer == null)
        {
            return;
        }

        if (sortedPlayers.Length > 0)
        {
            var firstPlayerData = sortedPlayers[0];
            UpdateFirstPlayerDisplay(firstPlayer, firstPlayerData.PlayerId, firstPlayerData.Score, 1);
        }

        if (scoreBoard == null)
        {
            return;
        }

        // Mettre à jour les autres joueurs (sans image)
        for (int i = 1; i < sortedPlayers.Length && i <= 7; i++)
        {
            var playerData = sortedPlayers[i];

            if (i - 1 < scoreBoard.childCount)  // Vérifie si l'élément existe déjà
            {
                Transform playerScore = scoreBoard.GetChild(i - 1);
                UpdatePlayerDisplay(playerScore, playerData.PlayerId, playerData.Score, i + 1);
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
            return;
        }

        playerScore.gameObject.SetActive(true);
        SetText(playerScore, "PosImage/PosText", rank.ToString());

        // Récupérer le pseudo du joueur à partir de PlayerDataManager
        string playerName = (PlayerDataManager.Datainstance?.GetPlayerData(playerId)?.pseudo).ToString() ?? "Player " + playerId;
        SetText(playerScore, "PlayerImage/PlayerNameText", playerName);

        SetText(playerScore, "ScoreImage/ScoreText", score.ToString());
    }

    private void UpdateFirstPlayerDisplay(Transform playerDisplay, ulong playerId, int score, int rank)
    {
        if (playerDisplay == null)
        {
            return;
        }

        playerDisplay.gameObject.SetActive(true);
        SetText(playerDisplay, "PlayerPosImage/PosText", rank.ToString());

        // Récupérer le pseudo du joueur à partir de PlayerDataManager
        string playerName = (PlayerDataManager.Datainstance?.GetPlayerData(playerId).pseudo).ToString() ?? "Player " + playerId;
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
        }
    }

    private void SetText(Transform parent, string path, string value)
    {
        Transform target = parent.Find(path);
        if (target == null)
        {
            return;
        }

        if (target.TryGetComponent<TextMeshProUGUI>(out var textComponent))
        {
            textComponent.text = value;
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