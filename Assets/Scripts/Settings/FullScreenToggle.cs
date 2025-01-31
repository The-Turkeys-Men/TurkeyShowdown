using UnityEngine;
using UnityEngine.UI;



public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private Toggle _toggle; 

    void Start()
    {
        _toggle.isOn = Screen.fullScreen;
        _toggle.onValueChanged.AddListener(SetFullscreen);
    }

    void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}

