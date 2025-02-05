using System;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Grappler : NetworkBehaviour
{   
    [SerializeField] private GameObject _tete;
    [SerializeField] private GameObject _teteGrapain;

    [SerializeField] private Transform _startGrabPoint;
    [SerializeField] private LineRenderer _grappleVisual;

    [SerializeField] private float _startWidth = 0f;
    [SerializeField] private float _endWidth = 1f;
    [SerializeField] private float _grappleRange = 5;
    private Vector3 _hitGrapPosition;
    private Quaternion _hitGrapRotation;
    
     

    private Rigidbody2D _rb;

    private bool _isGripped;

    private Vector2 _grappledPoint;
    private float _grappleDistance;
    private GameObject _curentGrabTete;
    private Vector2 _wallNormal;
    private float _wallAngle;
    private float _wallHeadOffset = -0.5f;
    private float _wallNeckOffset = -0.35f;
    [SerializeField] private Transform _neckStartPoint;


    void Start()
    {
        InitializeWidthLineRenderer();
        _rb = GetComponent<Rigidbody2D>();
        _isGripped = false;
        _grappleVisual.positionCount = 3;
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
        float magnitude = vector.magnitude;
        Vector2 projectedVector = vector - Vector2.Dot(vector, normal) * normal;
        return projectedVector.normalized * magnitude;
    }

    public void TryGrab(Vector2 grabDirection)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, grabDirection, _grappleRange, 1 << 6);
        if (hitInfo)
        {
            _wallNormal = hitInfo.normal;
            _wallAngle = Mathf.Atan2(_wallNormal.x, -_wallNormal.y) * Mathf.Rad2Deg;
            StartGrab(hitInfo.point);
            
            _hitGrapPosition=hitInfo.point - _wallNormal * _wallHeadOffset;
            _hitGrapRotation = Quaternion.Euler(0f, 0f, _wallAngle);
            _curentGrabTete = Instantiate(_teteGrapain,_hitGrapPosition,_hitGrapRotation);
            _curentGrabTete.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

    }

    

    private void StartGrab(Vector2 hitPoint)
    {
        _isGripped = true;
        _grappleDistance = Vector2.Distance(transform.position, hitPoint);
        _grappledPoint = hitPoint;
        SwitchGrabVisualEffect(hitPoint, true);
        SwitchGrabVisualEffectServerRpc(hitPoint, true);
       
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
        SwitchGrabVisualEffect(Vector2.zero, false);
        SwitchGrabVisualEffectServerRpc(Vector2.zero, false);
        _isGripped = false;
        Destroy(_curentGrabTete);
        
    }

    private void GrabUpdate()
    {
        float distanceWithVelocity = Vector2.Distance((Vector2)transform.position + _rb.linearVelocity, _grappledPoint);
        float distanceWithInverseVelocity = Vector2.Distance((Vector2)transform.position - _rb.linearVelocity, _grappledPoint);
        float threshold = 0.1f;
        if (distanceWithVelocity > distanceWithInverseVelocity - threshold)
        {
            _rb.position = _grappledPoint + (_rb.position - _grappledPoint).normalized * _grappleDistance;
            _rb.linearVelocity = ProjectOnPlane(_rb.linearVelocity, _grappledPoint - (Vector2)transform.position);
        }
        else
        {
            _grappleDistance = Vector2.Distance(transform.position, _grappledPoint);
        }
        _grappleVisual.SetPosition(0, _startGrabPoint.position);
        UpdateGrabVisualEffectServerRpc();
        
        _curentGrabTete.transform.eulerAngles = Vector3.forward * _wallAngle;
        
        Debug.Log(_wallNormal);
        

        
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SwitchGrabVisualEffectServerRpc(Vector2 hitPoint, bool activated)
    {
        SwitchGrabVisualEffectClientRpc(hitPoint, activated);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchGrabVisualEffectClientRpc(Vector2 hitPoint, bool activated)
    {
        SwitchGrabVisualEffect(hitPoint, activated);
    }
    
    private void SwitchGrabVisualEffect(Vector2 hitPoint, bool activated)
    {
        _grappleVisual.enabled = activated;
        _grappleVisual.SetPosition(2, hitPoint- _wallNormal * _wallNeckOffset);
        _tete.SetActive(true);
        
    }
    
    [Rpc(SendTo.Server)]
    private void UpdateGrabVisualEffectServerRpc()
    {
        UpdateGrabVisualEffectClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateGrabVisualEffectClientRpc()
    {
        _grappleVisual.SetPosition(0, _startGrabPoint.position);
        _grappleVisual.SetPosition(1, _neckStartPoint.position);
        _tete.SetActive(false);
        _grappleVisual.endWidth=0.08f;
        _grappleVisual.startWidth=0.05f;
        
        

    }

    private void InitializeWidthLineRenderer()
    {
        var curve = new AnimationCurve();
        curve.AddKey(1, _startWidth);
        curve.AddKey(0, _endWidth);
        _grappleVisual.widthCurve = curve;
    }
}
