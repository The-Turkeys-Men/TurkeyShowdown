using TMPro;
using UnityEngine;

public class TimerText : MonoBehaviour
{
    private IGameModeManager _gameModeManager;
    [SerializeField] private TextMeshProUGUI _timerText;

    private Color _defaultTimerColor;
    private Color _feedbackTimerColor;

    [SerializeField] private float _valueVolume = 1f;

    private float _nextTickTime = 0f;
    private float _tickInterval = 1f;



    private void Start()
    {
        _gameModeManager = DeathMatchManager.GetInstance();
        _defaultTimerColor = Color.white;
        _feedbackTimerColor = new Color32(217, 45, 45, 255);

        Debug.Log(_feedbackTimerColor);
    }

    private void Update()
    {
        _timerText.text = $"{_gameModeManager.TimeLeft.Value / 60:D2}:{_gameModeManager.TimeLeft.Value % 60:D2}";
        FeedbackVFXTimer();
        FeedbackSFXTimer();
    }

    private void FeedbackVFXTimer()
    {

        if (_gameModeManager.TimeLeft.Value <= 10)
        {
            if (_gameModeManager.TimeLeft.Value % 2 == 0)
            {
                _timerText.color = _feedbackTimerColor;

            }
            else
            {
                _timerText.color = _defaultTimerColor;
            }

        }
    }

    private void FeedbackSFXTimer()
    {
        if (!_gameModeManager.IsGameActive)
        {
            return;
        }

        if (_gameModeManager.TimeLeft.Value <= 10 && _gameModeManager.TimeLeft.Value % 2 == 0 && Time.time >= _nextTickTime)
        {
            AudioManager.Instance.PlaySFX("tickClock", transform.position, _valueVolume, false);
            _nextTickTime = Time.time + _tickInterval;
        }
    }
}
