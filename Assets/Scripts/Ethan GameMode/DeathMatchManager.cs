using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatchManager : NetworkBehaviour, IGameModeManager
{
    [SerializeField] private int maxGameTime = 300;
    [SerializeField] private int scoreToWin = 10;
    [SerializeField] private float disconnectDelay = 30f;

    public NetworkVariable<int> TimeLeft { get; set; } = new();
    public int MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }

    public NetworkVariable<Dictionary<ulong, int>> PlayerScores { get; set; } = new(new Dictionary<ulong, int>());

    private bool isGameActive = false;
    private bool isServerReady = true; // Flag to track if the server is ready to accept new connections
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button disconnectButton;

    private const ulong NoWinner = ulong.MaxValue; // Default value to represent no winner

    private float _timeLeftTimer = 1;

    private void Initialize()
    {
        MaxGameTime = maxGameTime;
        ScoreToWin = scoreToWin;
        TimeLeft.Value = maxGameTime;

        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }

        if (disconnectButton != null)
        {
            disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
            disconnectButton.gameObject.SetActive(false);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Initialize();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            PlayerScores.Value = new Dictionary<ulong, int>();
        }
        else if (IsClient)
        {
            Debug.Log("Client is connected to the server.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!isServerReady)
            {
                Debug.Log($"Server is not ready to accept new connections. Rejecting client {clientId}.");
                NetworkManager.Singleton.DisconnectClient(clientId);
                return;
            }

            if (!PlayerScores.Value.ContainsKey(clientId))
            {
                PlayerScores.Value[clientId] = 0;
                PlayerScores.SetDirty(true);
                Debug.Log($"Client {clientId} has been added to the scores dictionary with an initial score of 0.");
            }

            if (!isGameActive)
            {
                StartGame();
            }
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (IsServer)
        {
            if (PlayerScores.Value.ContainsKey(clientId))
            {
                PlayerScores.Value.Remove(clientId);
                PlayerScores.SetDirty(true);
            }

            if (NetworkManager.Singleton.ConnectedClients.Count == 0)
            {
                ResetServer();
            }
        }
    }

    private void StartGame()
    {
        isGameActive = true;
        TimeLeft.Value = MaxGameTime;
    }

    private void ResetServer()
    {
        isGameActive = false;
        isServerReady = true; // Server is ready to accept new connections again
        TimeLeft.Value = MaxGameTime;
        PlayerScores.Value.Clear();
        PlayerScores.SetDirty(true);

        if (scorePanel != null) scorePanel.SetActive(false);
        if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (!isGameActive) return;

        _timeLeftTimer -= Time.deltaTime;
        if (_timeLeftTimer <= 0)
        {
            TimeLeft.Value--;
            TimeLeft.SetDirty(true);
            _timeLeftTimer = 1;
        }
        
        if (TimeLeft.Value <= 0)
        {
            TimeLeft.Value = 0;
            OnLose();
        }
    }

    public void OnPlayerKill(ulong killerId)
    {
        if (!isGameActive) return;

        if (PlayerScores.Value.ContainsKey(killerId))
        {
            PlayerScores.Value[killerId]++;
            PlayerScores.SetDirty(true);

            if (PlayerScores.Value[killerId] >= ScoreToWin)
            {
                OnWin(killerId);
            }
        }
    }

    public void OnWin(ulong winnerId)
    {
        EndGame(winnerId);
    }

    public void OnLose()
    {
        // Find the player with the highest score
        ulong bestPlayerId = NoWinner;
        int bestScore = 0;

        foreach (var player in PlayerScores.Value)
        {
            if (player.Value > bestScore)
            {
                bestScore = player.Value;
                bestPlayerId = player.Key;
            }
        }

        // If a player has a score greater than 0, declare them the winner
        if (bestScore > 0)
        {
            OnWin(bestPlayerId);
        }
        else
        {
            EndGame(NoWinner); // Draw
        }
    }

    private void EndGame(ulong winnerId)
    {
        isGameActive = false;
        isServerReady = false; // Server is not ready to accept new connections
        if (IsServer)
        {
            ShowScorePanelClientRpc(winnerId);
            StartCoroutine(AutoDisconnectPlayers());
        }
    }

    private System.Collections.IEnumerator AutoDisconnectPlayers()
    {
        yield return new WaitForSeconds(disconnectDelay);
        if (IsServer)
        {
            // Hide the panel on the client side
            HideScorePanelClientRpc();

            // Create a copy of the client list to avoid modification during enumeration
            var clients = new List<ulong>(NetworkManager.Singleton.ConnectedClients.Keys);
            foreach (var clientId in clients)
            {
                NetworkManager.Singleton.DisconnectClient(clientId);
            }
            ResetServer();
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateTimer();
        }

        if (!isGameActive) return;

    }

    /*[Rpc(SendTo.Server, RequireOwnership = false)]
    private void AddKillForSelfServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer)
        {
            Debug.LogWarning("RPC was called, but this is not the server.");
            return;
        }

        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Server received a request to add a kill for client {clientId}.");

        if (PlayerScores.Value.ContainsKey(clientId))
        {
            PlayerScores.Value[clientId]++;
            PlayerScores.SetDirty(true);
            Debug.Log($"Client {clientId}'s score has been updated to {PlayerScores.Value[clientId]}.");

            if (PlayerScores.Value[clientId] >= ScoreToWin)
            {
                OnWin(clientId);
            }
        }
        else
        {
            Debug.LogWarning($"Client {clientId} does not exist in the scores dictionary.");
        }
    }*/

    private void UpdateScoreDisplay(ulong winnerId)
    {
        if (scorePanel == null) return;
        TextMeshProUGUI scoreDisplayText = scorePanel.GetComponentInChildren<TextMeshProUGUI>();
        if (scoreDisplayText == null) return;

        List<KeyValuePair<ulong, int>> sortedPlayers = new(PlayerScores.Value);
        sortedPlayers.Sort((a, b) => b.Value.CompareTo(a.Value));

        string scoreText = "Scores :\n";
        foreach (var player in sortedPlayers)
        {
            scoreText += $"Player {player.Key} : {player.Value} points\n";
        }
        scoreDisplayText.text = scoreText;

        if (resultText != null)
        {
            if (winnerId != NoWinner && winnerId == NetworkManager.Singleton.LocalClientId)
            {
                resultText.text = "Tu a Gagné !";
            }
            else if (winnerId != NoWinner)
            {
                resultText.text = "Tu a Perdu !";
            }
            else
            {
                resultText.text = "Match Nul !";
            }
        }
    }

    private void OnDisconnectButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        if (scorePanel != null) scorePanel.SetActive(false);
        if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShowScorePanelClientRpc(ulong winnerId)
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);
            UpdateScoreDisplay(winnerId);
        }
        if (disconnectButton != null)
        {
            disconnectButton.gameObject.SetActive(true);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void HideScorePanelClientRpc()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(false);
        }
        if (disconnectButton != null)
        {
            disconnectButton.gameObject.SetActive(false);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            ResetServer();
        }
    }
}