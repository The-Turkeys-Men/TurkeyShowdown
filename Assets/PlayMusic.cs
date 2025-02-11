using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    [SerializeField]float baseVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlayMusic(transform.position,baseVolume);
    }

    
}
