using UnityEngine;

public class PanelVolume : MonoBehaviour
{
    public GameObject PanelAudio;
    public void ActivePanelVolume()
    {
        PanelAudio.SetActive(!PanelAudio.activeSelf);
    }
}
