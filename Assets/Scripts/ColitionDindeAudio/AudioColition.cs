using UnityEngine;

public class AudioColition : MonoBehaviour
{
    private Collider2D AudioColideur;
    [SerializeField] float baseVolume;
    
    void Start()
    {
        AudioColideur=GetComponent<Collider2D>();
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySFX("colitionMurPlayer",transform.position,baseVolume);
    }
}
