using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class LeaderBoardHUDManager : NetworkBehaviour
{
    public static LeaderBoardHUDManager Instance;
    private LeaderBoardHUDPanel _panel;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            Debug.Log("[LeaderBoardHUDManager] Instance created.");
        }
        else
        {
            Debug.LogWarning("[LeaderBoardHUDManager] Duplicate instance detected. Destroying the new one.");
            DestroyImmediate(gameObject);
        }
    }

    public void SetPanel(LeaderBoardHUDPanel newPanel)
    {
        _panel = newPanel;
        Debug.Log("[LeaderBoardHUDManager] Panel assigned.");
        UpdateLeaderboardUI();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Debug.Log("[LeaderBoardHUDManager] Client spawned. Listening for score changes.");
            DeathMatchManager.GetInstance().PlayerScores.OnValueChanged += OnScoresChanged;
        }
    }

    private void OnScoresChanged(List<PlayerScore> previousScores, List<PlayerScore> newScores)
    {
        Debug.Log("[LeaderBoardHUDManager] Scores changed. Updating UI.");
        UpdateLeaderboardUI();
    }

    public void UpdateLeaderboardUI()
    {
        if (_panel == null)
        {
            Debug.LogError("[LeaderBoardHUDManager] Panel not assigned!");
            return;
        }
        
        List<PlayerScore> playerScores = DeathMatchManager.GetInstance().PlayerScores.Value;
        if (playerScores.Count == 0)
        {
            Debug.Log("[LeaderBoardHUDManager] No players found.");
            _panel.FirstPlaceText.text = "No players";
            _panel.CurrentPlaceText.text = "Unranked";
            return;
        }

        playerScores = playerScores.OrderByDescending(ps => ps.Score).ToList();
        
        foreach (var playerScore in playerScores.ToList())
        {
            Debug.Log($"Pseudo detected with id {playerScore.PlayerId}: " + PlayerDataManager.Datainstance?.GetPlayerData(playerScore.PlayerId).pseudo);
        }

        ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
        int playerRank = playerScores.FindIndex(ps => ps.PlayerId == localPlayerId);

        // Récupérer le pseudo du joueur à partir de PlayerDataManager
        string playerName = (PlayerDataManager.Datainstance?.GetPlayerData(localPlayerId).pseudo).ToString();

        _panel.CurrentPlaceText.text = playerRank == -1 
            ? "Unranked" 
            : $"#{playerRank + 1} {playerName} - {playerScores[playerRank].Score}";

        string firstPlaceName = (PlayerDataManager.Datainstance?.GetPlayerData(playerScores[0].PlayerId).pseudo).ToString();
        string secondPlaceName = (PlayerDataManager.Datainstance?.GetPlayerData(playerScores[1].PlayerId).pseudo).ToString();
        _panel.FirstPlaceText.text = playerRank == 0 && playerScores.Count > 1 
            ? $"#2 {secondPlaceName} - {playerScores[1].Score}" 
            : $"#1 {firstPlaceName} - {playerScores[0].Score}";

        UpdateLeaderboardClientRpc(_panel.FirstPlaceText.text, _panel.CurrentPlaceText.text);
    }

    [ClientRpc]
    private void UpdateLeaderboardClientRpc(string firstPlaceText, string currentPlaceText)
    {
        if (!IsClient) return;
        Debug.Log("[LeaderBoardHUDManager] Updating leaderboard UI on client.");
        if (_panel != null)
        {
            _panel.FirstPlaceText.text = firstPlaceText;
            _panel.CurrentPlaceText.text = currentPlaceText;
        }
    }

    public void ResetLeaderboard()
    {
        if (_panel != null)
        {
            Debug.Log("[LeaderBoardHUDManager] Resetting leaderboard.");
            _panel.FirstPlaceText.text = "No players";
            _panel.CurrentPlaceText.text = "Unranked";
        }
    }
}