using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private IGameModeManager _gameModeManager;
    [SerializeField] private TextMeshProUGUI _timerText;

    private void Awake()
    {
        _gameModeManager = FindAnyObjectByType<DeathMatchManager>();
    }

    private void Update()
    {
        _timerText.text = $"{Mathf.FloorToInt(_gameModeManager.TimeLeft / 60)}:{(_gameModeManager.TimeLeft % 60).ToString("00")}";
    }
}
