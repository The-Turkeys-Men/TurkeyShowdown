using Unity.Netcode;
using UnityEngine;

public class Grappler : NetworkBehaviour
{
    [SerializeField] private Transform _startGrabPoint;
    [SerializeField] private LineRenderer _grappleVisual;

    [SerializeField] private float _startWidth = 0f;
    [SerializeField] private float _endWidth = 1f;
    [SerializeField] private float _grappleRange = 5;

    private Rigidbody2D _rb;

    private bool _isGripped;

    private Vector2 _grappledPoint;

    void Start()
    {
        InitializeWidthLineRenderer();
        _rb = GetComponent<Rigidbody2D>();
        _isGripped = false;
    }

    void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_isGripped)
        {
            GrabUpdate();
        }
    }

    private Vector2 ProjectOnPlane(Vector2 vector, Vector2 normal)
    {
        normal.Normalize();
        return vector - Vector2.Dot(vector, normal) * normal;
    }

    public void TryGrab(Vector2 grabDirection)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, grabDirection, _grappleRange, 1 << 6);
        if (hitInfo)
        {
            StartGrab(hitInfo.point);
        }

    }

    private void StartGrab(Vector2 hitPoint)
    {
        _isGripped = true;
        SwitchGrabVisualEffectClientRpc(hitPoint, true);
    }
    
    public void TryReleaseGrab()
    {
        if (!_isGripped)
        {
            return;
        }
        ReleaseGrab();
    }

    private void ReleaseGrab()
    {
        SwitchGrabVisualEffectClientRpc(Vector2.zero, false);
        _isGripped = false;
    }

    private void GrabUpdate()
    {
        
        float distanceWithVelocity = Vector2.Distance((Vector2)transform.position + _rb.linearVelocity, _grappledPoint);
        float distanceWithInverseVelocity = Vector2.Distance((Vector2)transform.position - _rb.linearVelocity, _grappledPoint);
        if (distanceWithVelocity > distanceWithInverseVelocity)
        {
            _rb.linearVelocity = ProjectOnPlane(_rb.linearVelocity, _grappledPoint - (Vector2)transform.position);
        }
        UpdateGrabVisualEffectClientRpc();
    }

    [ClientRpc]
    private void SwitchGrabVisualEffectClientRpc(Vector2 hitPoint, bool activated)
    {
        _grappleVisual.enabled = activated;
        _grappledPoint = hitPoint;
        _grappleVisual.SetPosition(0, hitPoint);
    }

    [ClientRpc]
    private void UpdateGrabVisualEffectClientRpc()
    {
        _grappleVisual.SetPosition(1, _startGrabPoint.position);
    }

    private void InitializeWidthLineRenderer()
    {
        var curve = new AnimationCurve();
        curve.AddKey(1, _startWidth);
        curve.AddKey(0, _endWidth);
        _grappleVisual.widthCurve = curve;
    }
}
