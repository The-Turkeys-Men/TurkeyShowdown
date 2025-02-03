using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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

    private void PlaySound( Sound[] soundList, string name,Vector3 pos)
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
            audioSource.spatialBlend=1f;
            audioSource.clip = sound.Clip;
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
        PlaySound( MusicSounds, name,pos);
    }

    public void PlaySFX(string name,Vector3 pos)
    {
        PlaySound( SfxSounds, name, pos);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s.Name == name);
    }
}