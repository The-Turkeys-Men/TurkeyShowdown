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
        _timerText.text = _gameModeManager.TimeLeft.ToString("F2");
    }
}
