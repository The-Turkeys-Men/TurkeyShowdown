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
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void SetPanel(LeaderBoardHUDPanel newPanel)
    {
        _panel = newPanel;
        UpdateLeaderboardUI();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            DeathMatchManager.GetInstance().PlayerScores.OnValueChanged += OnScoresChanged;
        }
    }

    private void OnScoresChanged(List<PlayerScore> previousScores, List<PlayerScore> newScores)
    {
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
            _panel.FirstPlaceText.text = "No players";
            _panel.CurrentPlaceText.text = "Unranked";
            return;
        }

        playerScores = playerScores.OrderByDescending(ps => ps.Score).ToList();

        ulong localPlayerId = NetworkManager.Singleton.LocalClientId;
        int playerRank = playerScores.FindIndex(ps => ps.PlayerId == localPlayerId);

        _panel.CurrentPlaceText.text = playerRank == -1 
            ? "Unranked" 
            : $"#{playerRank + 1} {playerScores[playerRank].PlayerName} - {playerScores[playerRank].Score}";

        _panel.FirstPlaceText.text = playerRank == 0 && playerScores.Count > 1 
            ? $"#2 {playerScores[1].PlayerName} - {playerScores[1].Score}" 
            : $"#1 {playerScores[0].PlayerName} - {playerScores[0].Score}";

        UpdateLeaderboardClientRpc(_panel.FirstPlaceText.text, _panel.CurrentPlaceText.text);
    }

    [ClientRpc]
    private void UpdateLeaderboardClientRpc(string firstPlaceText, string currentPlaceText)
    {
        if (!IsClient) return;
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
            _panel.FirstPlaceText.text = "No players";
            _panel.CurrentPlaceText.text = "Unranked";
        }
    }
}
