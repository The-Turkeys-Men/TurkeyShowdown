using System.Collections.Generic;
using Unity.Netcode;

public interface IGameModeManager
{
    public static IGameModeManager GetInstance()
    {
        return null;
    }
    public NetworkVariable<int> TimeLeft { get; set; }
    public int MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }
    public NetworkVariable<List<PlayerScore>> PlayerScores { get; set; }

    public bool IsGameActive { get; }
    public void OnWin(ulong winnerId);
    public void OnLose();
}
