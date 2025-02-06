using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] protected AudioMixerGroup _audioMixerSFX;
    [SerializeField] protected AudioMixerGroup _audioMixerMusic;
    public AudioSource AudioSourceMusic;
    public AudioSource AudioSourceSFX;
   

    public Sound[] MusicSounds;
    public Sound[] SfxSounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlaySound( Sound[] soundList, string name,Vector3 pos ,AudioMixerGroup Volume,float sonsLocale)
    {
        Sound sound = Array.Find(soundList, s => s.Name == name);
    
        if (sound == null)
        {
            Debug.Log($"{name} Not Found");
        }
        else
        {
            GameObject tempAudio=new GameObject("tempAudio");
            tempAudio.transform.position=pos;
            AudioSource audioSource= tempAudio.AddComponent<AudioSource>();
            audioSource.spatialBlend=sonsLocale;
            audioSource.clip = sound.Clip;
            audioSource.outputAudioMixerGroup=Volume;
            audioSource.Play();
            Destroy(tempAudio,sound.Clip.length);
            Debug.Log(audioSource.clip.name);
        }
            
        
        
        
    }



    private void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void StopMusic()
    {
        StopSound(AudioSourceMusic);
    }

    public void StopSFX()
    {
        StopSound(AudioSourceSFX);
    }

    public void PlayMusic(string name,Vector3 pos)
    {
        float sonsLocale=0f;
        AudioMixerGroup Volume= _audioMixerMusic;
        PlaySound( MusicSounds, name,pos,Volume,sonsLocale);
    }

    public void PlaySFX(string name,Vector3 pos)
    {
        float sonsLocale=1f;
        AudioMixerGroup Volume=_audioMixerSFX;
        PlaySound( SfxSounds, name, pos,Volume,sonsLocale);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s.Name == name);
    }
}