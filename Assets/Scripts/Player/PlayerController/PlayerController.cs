using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private PlayerWeapon _playerWeapon;
    [SerializeField] private Grappler _playerGrappler;

    [SerializeField] private Transform _rotationPivot;

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector2 screenMousePos = Input.mousePosition;
        Vector2 direction = _camera.ScreenToWorldPoint(screenMousePos) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        UpdateRotationServerRpc(angle);
    }
    
    [ServerRpc]
    private void UpdateRotationServerRpc(float angle)
    {
        UpdateRotationClientRpc(angle);
    }

    [ClientRpc]
    private void UpdateRotationClientRpc(float angle)
    {
        _rotationPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    #region Movement

    public void OnMove(Vector2 direction)
    {
        _playerMovement.TryMove(direction);
    }
    
    #endregion

    #region Grapple
    
    public void OnGrapple()
    {
        Vector2 screenMousePos = Input.mousePosition;
        Vector2 direction = _camera.ScreenToWorldPoint(screenMousePos) - transform.position;
        _playerGrappler.TryGrab(direction);
    }

    public void OnUnGrapple()
    {
        _playerGrappler.TryReleaseGrab();
    }
    #endregion
    
    #region Weapon

    public void OnShoot()
    {
        _playerWeapon.tryShoot();
    }

    public void OnThrowOrGrab()
    {
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
