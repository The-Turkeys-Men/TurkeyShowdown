using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource _audioSourceMusic;
    public AudioSource _audioSourceSFX;

    public Sound[] _musicSounds;
    public Sound[] _sfxSounds;

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
        Sound sound = Array.Find(soundList, s => s._name == name);
    
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
            audioSource.clip = sound._clip;
            audioSource.Play();
            Destroy(tempAudio,sound._clip.length);
            Debug.Log(audioSource.clip.name);
        }
            
        
        
        
    }



    private void StopSound(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void StopMusic()
    {
        StopSound(_audioSourceMusic);
    }

    public void StopSFX()
    {
        StopSound(_audioSourceSFX);
    }

    public void PlayMusic(string name,Vector3 pos)
    {
        PlaySound( _musicSounds, name,pos);
    }

    public void PlaySFX(string name,Vector3 pos)
    {
        PlaySound( _sfxSounds, name, pos);
    }

    public bool IsSoundInList(Sound[] soundList, string name)
    {
        return Array.Exists(soundList, s => s._name == name);
    }
}