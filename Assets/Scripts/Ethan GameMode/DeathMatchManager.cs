using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatchManager : NetworkBehaviour, IGameModeManager
{
    [SerializeField] private float maxGameTime = 300f;
    [SerializeField] private int scoreToWin = 10;

    public float TimeLeft { get; set; }
    public float MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }

    public NetworkVariable<Dictionary<ulong, int>> PlayerScores { get; set; } = new NetworkVariable<Dictionary<ulong, int>>(new Dictionary<ulong, int>());

    private bool isGameActive = false;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private Button disconnectButton;

    private void Awake()
    {
        MaxGameTime = maxGameTime;
        ScoreToWin = scoreToWin;
        TimeLeft = maxGameTime;

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
            // Inscription aux callbacks de connexion/déconnexion des clients
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            
            // Réinitialiser les scores à chaque démarrage du serveur
            PlayerScores.Value = new Dictionary<ulong, int>();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            // Ajouter un nouveau joueur et initialiser son score
            if (!PlayerScores.Value.ContainsKey(clientId))
            {
                PlayerScores.Value[clientId] = 0;
                PlayerScores.SetDirty(true);
            }

            // Lancer la partie si ce n'est pas encore fait
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
            // Retirer le joueur déconnecté et mettre à jour les scores
            if (PlayerScores.Value.ContainsKey(clientId))
            {
                PlayerScores.Value.Remove(clientId);
                PlayerScores.SetDirty(true);
            }

            // Si tous les joueurs se déconnectent, réinitialiser le serveur
            if (NetworkManager.Singleton.ConnectedClients.Count == 0)
            {
                ResetServer();
            }
        }
    }

    private void StartGame()
    {
        isGameActive = true;
        TimeLeft = MaxGameTime;
    }

    private void ResetServer()
    {
        isGameActive = false;
        TimeLeft = MaxGameTime;
        PlayerScores.Value.Clear(); // Effacer les scores des joueurs
        PlayerScores.SetDirty(true);

        // Réinitialiser l'état du jeu et les scores
        if (scorePanel != null) scorePanel.SetActive(false);
        if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
    }

    private void UpdateTimer()
    {
        if (!isGameActive) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0)
        {
            TimeLeft = 0;
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
                OnWin();
            }
        }
    }

    public void OnWin()
    {
        EndGame();
    }

    public void OnLose()
    {
        EndGame();
    }

    private void EndGame()
    {
        isGameActive = false;
        if (IsServer)
        {
            ShowScorePanelClientRpc();
        }
        PlayerScores.Value.Clear();  // Effacer les scores des joueurs à la fin de la partie
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateTimer();
        }

        if (!isGameActive) return;

        if (IsClient && Input.GetKeyDown(KeyCode.K))
        {
            AddKillForSelfServerRpc();
        }
    }

    [ServerRpc]
    private void AddKillForSelfServerRpc()
    {
        if (!IsServer) return;

        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if (PlayerScores.Value.ContainsKey(clientId))
        {
            PlayerScores.Value[clientId]++;
            PlayerScores.SetDirty(true);

            if (PlayerScores.Value[clientId] >= ScoreToWin)
            {
                OnWin();
            }
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scorePanel == null) return;
        TextMeshProUGUI scoreDisplayText = scorePanel.GetComponentInChildren<TextMeshProUGUI>();
        if (scoreDisplayText == null) return;

        List<KeyValuePair<ulong, int>> sortedPlayers = new(PlayerScores.Value);
        sortedPlayers.Sort((a, b) => b.Value.CompareTo(a.Value));

        string scoreText = "Scores :\n";
        foreach (var player in sortedPlayers)
        {
            scoreText += $"Joueur {player.Key} : {player.Value} points\n";
        }
        scoreDisplayText.text = scoreText;
    }

    private void OnDisconnectButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        if (scorePanel != null) scorePanel.SetActive(false);
        if (disconnectButton != null) disconnectButton.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void ShowScorePanelClientRpc()
    {
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);
            UpdateScoreDisplay();
        }
        if (disconnectButton != null)
        {
            disconnectButton.gameObject.SetActive(true);
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
