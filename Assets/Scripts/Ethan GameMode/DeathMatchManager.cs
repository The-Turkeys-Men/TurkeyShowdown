using System.Collections;
using System.Collections.Generic;
using MapVote;
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

    public NetworkVariable<List<PlayerScore>> PlayerScores { get; set; } = new(new List<PlayerScore>());

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
        foreach (var connectedClient in NetworkManager.ConnectedClients)
        {
            OnClientConnected(connectedClient.Key);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Initialize();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            PlayerScores.Value = new List<PlayerScore>();
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!PlayerScores.Value.Exists(ps => ps.PlayerId == clientId))
            {
                PlayerScores.Value.Add(new PlayerScore { PlayerId = clientId, Score = 0 });
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
            var playerScore = PlayerScores.Value.Find(ps => ps.PlayerId == clientId);
            if (playerScore.PlayerId == clientId)
            {
                PlayerScores.Value.Remove(playerScore);
                PlayerScores.SetDirty(true);
            }

            // Vérifier s'il n'y a plus de joueurs connectés
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
        if (NetworkManager.Singleton.ConnectedClients.Count == 0)
        {
            isGameActive = false;
            TimeLeft.Value = MaxGameTime;
            PlayerScores.Value.Clear();
            PlayerScores.SetDirty(true);
            Debug.Log("Server reset: No players connected.");
        }
        else
        {
            Debug.Log("Server not reset: Players are still connected.");
        }
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

        var playerScore = PlayerScores.Value.Find(ps => ps.PlayerId == killerId);
        if (playerScore.PlayerId == killerId)
        {
            playerScore.Score++;
            
            PlayerScores.SetDirty(true);

            Debug.Log(playerScore.Score + " points for player " + killerId);
            if (playerScore.Score >= ScoreToWin)
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

        foreach (var playerScore in PlayerScores.Value)
        {
            if (playerScore.Score > bestScore)
            {
                bestScore = playerScore.Score;
                bestPlayerId = playerScore.PlayerId;
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
            Debug.Log("[DeathMatchManager] EndGame called, showing score panel.");
            PlayerScore[] playerScoresArray = PlayerScores.Value.ToArray();

            if (ScorePanelManager.Instance != null)
            {
                Debug.Log($"[DeathMatchManager] Calling ShowScorePanelClientRpc with {playerScoresArray.Length} players.");
                ScorePanelManager.Instance.ShowScorePanelClientRpc(winnerId, playerScoresArray);
                MapVoteManager.Instance?.StartMapVote();
            }
            else
            {
                Debug.LogError("[DeathMatchManager] ScorePanelManager.Instance is NULL!");
            }

            StartCoroutine(ResetGameAfterDelay(10f));
        }
    }

    private IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetServer();
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateTimer();
        }
    }
}