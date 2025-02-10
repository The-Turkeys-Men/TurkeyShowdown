using DG.Tweening;
using UnityEngine;

public class ObstacleOpacityManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sR;
    [SerializeField] private float _timeFading = 0.5f;

    private Color _defaultColor;
    private Color _fadedColor;
    private float _valueAlphaColor = 0.4f;


    private void Start()
    {
        _sR = GetComponent<SpriteRenderer>();
        _defaultColor = _sR.color;
        _fadedColor = _sR.color;
        _fadedColor.a = _valueAlphaColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            FadeOut();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            FadeIn();
        }
    }

    private void FadeIn()
    {
        _sR.DOFade(1, _timeFading);
    }

    private void FadeOut() 
    {
        _sR.DOFade(_valueAlphaColor, _timeFading);
    }

}
