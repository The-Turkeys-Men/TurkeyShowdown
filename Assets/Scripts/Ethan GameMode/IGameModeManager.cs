using Unity.Netcode;

public interface IGameModeManager
{
    float TimeLeft { get; set; }
    float MaxGameTime { get; set; }
    int ScoreToWin { get; set; }

    void OnWin();
    void OnLose();
}
