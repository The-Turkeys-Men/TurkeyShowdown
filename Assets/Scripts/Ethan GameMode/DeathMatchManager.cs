using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeathMatchManager : NetworkBehaviour, IGameModeManager
{
    private static DeathMatchManager _instance;
    [SerializeField] private int maxGameTime = 300;
    [SerializeField] private int scoreToWin = 10;

    public NetworkVariable<int> TimeLeft { get; set; } = new();
    public int MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }

    public NetworkVariable<Dictionary<ulong, int>> PlayerScores { get; set; } = new(new Dictionary<ulong, int>());

    private bool isGameActive = false;
    private const ulong NoWinner = ulong.MaxValue;

    private float _timeLeftTimer = 1;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public static IGameModeManager GetInstance()
    {
        return _instance;
    }

    private void Initialize()
    {
        MaxGameTime = maxGameTime;
        ScoreToWin = scoreToWin;
        TimeLeft.Value = maxGameTime;
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
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!PlayerScores.Value.ContainsKey(clientId))
            {
                PlayerScores.Value[clientId] = 0;
                PlayerScores.SetDirty(true);
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
        TimeLeft.Value = MaxGameTime;
        PlayerScores.Value.Clear();
        PlayerScores.SetDirty(true);
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

        if (bestScore > 0)
        {
            OnWin(bestPlayerId);
        }
        else
        {
            EndGame(NoWinner);
        }
    }

    public void EndGame(ulong winnerId)
    {
        isGameActive = false;
        if (IsServer)
        {
            Debug.Log("EndGame called, showing score panel.");
            ScorePanelManager.Instance.ShowScorePanel(winnerId, PlayerScores.Value);
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateTimer();
        }
    }
}