using System.Collections.Generic;
using Unity.Netcode;

public interface IGameModeManager
{
    public NetworkVariable<int> TimeLeft { get; set; }
    public int MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }
    public NetworkVariable<Dictionary<ulong, int>> PlayerScores { get; set; }

    public void OnWin(ulong winnerId);
    public void OnLose();
}
