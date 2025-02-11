using UnityEditor.SceneManagement;
using UnityEngine;

public class AudioColition : MonoBehaviour
{
    private Collider2D AudioColideur;
    [SerializeField] float baseVolume;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioColideur=GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        AudioManager.Instance.PlaySFX("colitionMurPlayer",transform.position,baseVolume);
    }
}
