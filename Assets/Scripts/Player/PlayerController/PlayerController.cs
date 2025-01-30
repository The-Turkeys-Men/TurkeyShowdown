using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private Grappler _playerGrappler;

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
        //TryShoot
    }

    public void OnThrow()
    {
        //TryThrow
    }
    
    #endregion
}
