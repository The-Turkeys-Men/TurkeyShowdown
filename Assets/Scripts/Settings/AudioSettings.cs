using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider _masterVol;
    [SerializeField] private Slider _musicVol;
    [SerializeField] private Slider _sfxVol;
    [SerializeField] private AudioMixer _mainAudioMixer;
    public void Change_masterVolume()
    {
        _mainAudioMixer.SetFloat("Master", Mathf.Log10(_masterVol.value) * 20);
    }

    public void Change_musicVolume()
    {
        _mainAudioMixer.SetFloat("Music", Mathf.Log10(_musicVol.value) * 20);
    }

    public void Change_sfxVolume()
    {
        _mainAudioMixer.SetFloat("SFX", Mathf.Log10(_sfxVol.value) * 20);
    }
}
