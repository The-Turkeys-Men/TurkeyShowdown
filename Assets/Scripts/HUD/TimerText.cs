using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private IGameModeManager _gameModeManager;
    [SerializeField] private TextMeshProUGUI _timerText;

    private void Awake()
    {
        //todo: optimize this
        _gameModeManager = FindAnyObjectByType<DeathMatchManager>();
    }

    private void Update()
    {
        _timerText.text = $"{_gameModeManager.TimeLeft.Value / 60:D2}:{_gameModeManager.TimeLeft.Value % 60:D2}";
    }
}
