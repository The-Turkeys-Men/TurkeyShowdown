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
    private Vector3 _hitGrabPosition;
    private Quaternion _hitGrapRotation;
    
     

    private Rigidbody2D _rb;

    private bool _isGripped;

    private Vector2 _grappledPoint;
    private float _grappleDistance;
    private GrappleHead _curentGrabHead;
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
        RaycastHit2D[] hitInfos = Physics2D.RaycastAll(transform.position, grabDirection, _grappleRange, 1 << LayerMask.NameToLayer("World"));
        RaycastHit2D hitInfo = default;
        foreach (RaycastHit2D info in hitInfos)
        {
            if (info.collider && !info.collider.isTrigger)
            {
                hitInfo = info;
                break;
            }
        }
        if (hitInfo)
        {
            _wallNormal = hitInfo.normal;
            _wallAngle = Mathf.Atan2(_wallNormal.x, -_wallNormal.y) * Mathf.Rad2Deg;

            AudioManager.Instance.PlaySFX("grapain",transform.position);
            
            SpawnHead(hitInfo.point, _wallNormal, _wallAngle);
            SpawnHeadServerRpc(hitInfo.point, _wallNormal, _wallAngle);
            
            StartGrab(hitInfo.point);
        }

    }

    [Rpc(SendTo.Server)]
    private void SpawnHeadServerRpc(Vector2 hitPoint, Vector2 hitNormal, float wallAngle)
    {
        SpawnHeadClientRpc(hitPoint, hitNormal, wallAngle);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnHeadClientRpc(Vector2 hitPoint, Vector2 hitNormal, float wallAngle)
    {
        if (IsOwner)
        {
            return;
        }
        SpawnHead(hitPoint, hitNormal, wallAngle);
    }
    
    private void SpawnHead(Vector2 hitPoint, Vector2 hitNormal, float wallAngle)
    {
        _hitGrabPosition= hitPoint - hitNormal * _wallHeadOffset;
        _curentGrabHead = Instantiate(_teteGrapain,_tete.transform.position,_tete.transform.rotation).GetComponent<GrappleHead>();
        _curentGrabHead.GoToPoint(_hitGrabPosition, wallAngle);
        _curentGrabHead.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        _curentGrabHead.BodyTransform = _neckStartPoint;
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
        
        _curentGrabHead.GoBackToBody();
        ReturnHeadServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void ReturnHeadServerRpc()
    {
        ReturnHeadClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ReturnHeadClientRpc()
    {
        if (IsOwner)
        {
            return;
        }
        _curentGrabHead.GoBackToBody();
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
        _grappleVisual.SetPosition(1, _neckStartPoint.position);
        _grappleVisual.SetPosition(2, _curentGrabHead.NeckTransform.position);
        UpdateGrabVisualEffectServerRpc();
        
        _curentGrabHead.transform.eulerAngles = Vector3.forward * _wallAngle;
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
        if (activated)
        {
            _grappleVisual.SetPosition(2, _curentGrabHead.NeckTransform.position);
        }
        _tete.SetActive(true);
        AudioManager.Instance.PlaySFX("retirGrapain",transform.position);
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
        _grappleVisual.SetPosition(2, _curentGrabHead.NeckTransform.position);
        _tete.SetActive(false);
    }

    private void InitializeWidthLineRenderer()
    {
        var curve = new AnimationCurve();
        curve.AddKey(0, _startWidth);
        curve.AddKey(1, _startWidth);
        curve.AddKey(2, _endWidth);
        _grappleVisual.widthCurve = curve;
    }
}
