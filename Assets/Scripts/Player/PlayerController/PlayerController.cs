using System;
using Debugger;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerWeapon _playerWeapon;
    [SerializeField] private Grappler _playerGrappler;

    [SerializeField] private Transform _rotationPivot;

    [FormerlySerializedAs("_inputActivated")] public bool InputActivated = true;

    private void Update()
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        Vector2 screenMousePos = Input.mousePosition;
        Vector2 direction = _camera.ScreenToWorldPoint(screenMousePos) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        UpdateRotationServerRpc(angle);
    }
    
    [Rpc(SendTo.Server)]
    private void UpdateRotationServerRpc(float angle)
    {
        UpdateRotationClientRpc(angle);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateRotationClientRpc(float angle)
    {
        _rotationPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    #region Movement

    public void OnMove(Vector2 direction)
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        
        _playerMovement.TryMove(direction);
    }
    
    #endregion

    #region Grapple
    
    public void OnGrapple()
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        
        Vector2 screenMousePos = Input.mousePosition;
        Vector2 direction = _camera.ScreenToWorldPoint(screenMousePos) - transform.position;
        _playerGrappler.TryGrab(direction);
    }

    public void OnUnGrapple()
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        
        _playerGrappler.TryReleaseGrab();
    }
    #endregion
    
    #region Weapon

    public void OnShoot()
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        
        _playerWeapon.tryShoot();
    }

    public void OnThrowOrGrab()
    {
        if (!IsOwner || !InputActivated)
        {
            return;
        }
        
        if (_playerWeapon.EquipedWeapon && _playerWeapon.EquipedWeapon.CanBeThrowed)
        {
            _playerWeapon.TryThrowWeapon();
        }
        else
        {
            _playerWeapon.TryEquipWeapon();
        }
    }
    
    #endregion
}
