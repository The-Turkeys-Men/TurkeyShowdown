using System.Collections.Generic;
using Unity.Netcode;

public interface IGameModeManager
{
    public float TimeLeft { get; set; }
    public float MaxGameTime { get; set; }
    public int ScoreToWin { get; set; }
    public NetworkVariable<Dictionary<ulong, int>> PlayerScores { get; set; }

    public void OnWin();
    public void OnLose();
}
