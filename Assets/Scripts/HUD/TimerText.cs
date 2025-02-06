using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private IGameModeManager _gameModeManager;
    [SerializeField] private TextMeshProUGUI _timerText;

    private void Start()
    {
        _gameModeManager = DeathMatchManager.Instance;
    }

    private void Update()
    {
        _timerText.text = $"{_gameModeManager.TimeLeft.Value / 60:D2}:{_gameModeManager.TimeLeft.Value % 60:D2}";
    }
}
