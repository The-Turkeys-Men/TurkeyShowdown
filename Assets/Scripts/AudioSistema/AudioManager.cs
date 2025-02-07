using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] protected AudioMixerGroup _audioMixerSFX;
    [SerializeField] protected AudioMixerGroup _audioMixerMusic;
    public AudioSource AudioSourceMusic;
    public AudioSource AudioSourceSFX;
    public List<string> nomMusic;
   

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

    private void PlaySound( Sound[] soundList, string name,Vector3 pos ,AudioMixerGroup Volume,float sonsLocale,bool loop,float baseVolume )
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
            audioSource.volume=baseVolume;
            audioSource.loop=loop;
            audioSource.outputAudioMixerGroup=Volume;
            audioSource.Play();
            Destroy(tempAudio,sound.Clip.length);
          
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

    public void PlayMusic(Vector3 pos)
    {
        string name= nomMusic.PickRandom();
        float sonsLocale=0f;
        AudioMixerGroup Volume= _audioMixerMusic;
        float baseVolume= 0.404f;
        bool loop=true;
        PlaySound( MusicSounds, name,pos,Volume,sonsLocale,loop,baseVolume);
    }

    public void PlaySFX(string name,Vector3 pos)
    {
        float sonsLocale=1f;
        bool loop=false;
        AudioMixerGroup Volume=_audioMixerSFX;
        float baseVolume= 2f;
        PlaySound( SfxSounds, name, pos,Volume,sonsLocale,loop,baseVolume);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s.Name == name);
    }
}