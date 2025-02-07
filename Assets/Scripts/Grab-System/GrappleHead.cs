using System;
using System.Collections;
using UnityEngine;

public class GrappleHead : MonoBehaviour
{
    private Transform _transform;
    public Transform BodyTransform;
    public Transform NeckTransform;

    public float MoveToWallDelta = 0.1f;
    public float MoveToBodyDelta = 0.4f;
    
    public Action OnBackToBody;
    
    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    public void GoToPoint(Vector2 position, float wallAngle)
    {
        StartCoroutine(GoToPointCoroutine(position, wallAngle));
    }

    public void GoBackToBody()
    {
        StartCoroutine(GoBackToBodyCoroutine());
    }

    private IEnumerator GoToPointCoroutine(Vector2 position, float wallAngle)
    {
        while (Vector2.Distance(_transform.position, position) > 0.1f)
        {
            _transform.position = Vector2.MoveTowards(_transform.position, position, MoveToWallDelta);
            yield return null;
        }
        NeckTransform.rotation = Quaternion.Euler(0, 0, wallAngle);
    }

    private IEnumerator GoBackToBodyCoroutine()
    {
        while (Vector2.Distance(_transform.position, BodyTransform.position) > 0.1f)
        {
            _transform.position = Vector2.MoveTowards(_transform.position, BodyTransform.position, MoveToBodyDelta);
            yield return null;
        }
        OnBackToBody?.Invoke();
    }
}
