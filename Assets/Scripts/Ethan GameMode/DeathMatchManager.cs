using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeathMatchManager : NetworkBehaviour, IGameModeManager
{
    [SerializeField] private float maxGameTime = 300f; // Temps de jeu max (5 minutes)
    [SerializeField] private int scoreToWin = 10; // Score pour gagner

    public float TimeLeft { get; set; }
    public float MaxGameTime { get => maxGameTime; set => maxGameTime = value; }
    public int ScoreToWin { get => scoreToWin; set => scoreToWin = value; }

    public NetworkVariable<int> TimeLeftSync = new NetworkVariable<int>();

    // Définition de la structure pour les scores des joueurs
    public struct PlayerScoreEntry : INetworkSerializable
    {
        public ulong PlayerID;
        public int Score;

        public PlayerScoreEntry(ulong playerID, int score)
        {
            PlayerID = playerID;
            Score = score;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref Score);
        }
    }

    // Liste des scores synchronisée en réseau
    private NetworkList<PlayerScoreEntry> PlayerScores;

    private void Awake()
    {
        Debug.Log("DeathMatchManager Awake()");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            Debug.Log("🚀 Server initialized, starting the timer!");
            TimeLeft = MaxGameTime;
            TimeLeftSync.Value = (int)TimeLeft;

            // Initialisation correcte de la NetworkList
            PlayerScores = new NetworkList<PlayerScoreEntry>();

            InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
        }
    }

    private void UpdateTimer()
    {
        if (!IsServer) return;

        if (TimeLeft > 0)
        {
            TimeLeft--;
            TimeLeftSync.Value = (int)TimeLeft;
            Debug.Log($"🕒 Time Left: {TimeLeft}");
        }
        else
        {
            CancelInvoke(nameof(UpdateTimer));
            Debug.Log("⏳ Game Over!");
            OnLose();
        }
    }

    public void OnWin()
    {
        Debug.Log("🎉 OnWin");
    }

    public void OnLose()
    {
        Debug.Log("💀 OnLose");
    }

    public void OnPlayerKill(ulong playerID)
    {
        if (!IsServer) return;

        // 🔴 Correction du FindIndex qui n'existe pas sur NetworkList
        int playerIndex = -1;
        for (int i = 0; i < PlayerScores.Count; i++)
        {
            if (PlayerScores[i].PlayerID == playerID)
            {
                playerIndex = i;
                break;
            }
        }

        if (playerIndex >= 0)
        {
            // Incrémentation du score
            var updatedScore = PlayerScores[playerIndex].Score + 1;
            PlayerScores[playerIndex] = new PlayerScoreEntry(playerID, updatedScore);
        }
        else
        {
            // Ajouter un nouveau joueur avec 1 kill
            PlayerScores.Add(new PlayerScoreEntry(playerID, 1));
        }

        // Correction du log pour éviter une erreur en cas de premier kill
        int score = playerIndex >= 0 ? PlayerScores[playerIndex].Score : 1;
        Debug.Log($"🔫 Player {playerID} score: {score}");

        // Vérifier la victoire
        if (score >= ScoreToWin)
        {
            OnWin();
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        Debug.Log($"IsServer: {IsServer}");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (NetworkManager.Singleton.LocalClient != null)
            {
                OnPlayerKill(NetworkManager.Singleton.LocalClient.ClientId);
            }
        }
    }
}
