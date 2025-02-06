using UnityEngine;
using UnityEngine.Audio;

public class SliderSons : MonoBehaviour
{
    
    public AudioMixer audioMixer;
   
     public void setSons(float volume)
    {
        audioMixer.SetFloat("Master",volume);
    }
    public void setMusic(float volume)
    {
        audioMixer.SetFloat("Music",volume);
    } 
     public void setSFX(float volume)
    {
        audioMixer.SetFloat("SFX",volume);
    }
}

