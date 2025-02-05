using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private bool _isShootPressed;
    
    private void Update()
    {
        if (_isShootPressed)
        {
            playerController.OnShoot();
        }
    }

    #region Movement

   public  void OnMove(InputAction.CallbackContext context)
    {
        playerController.OnMove(context.ReadValue<Vector2>());
    }

    #endregion
    
    #region Grapple
    
    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerController.OnGrapple();
        }
        if (context.canceled)
        {
            playerController.OnUnGrapple();
        }
    }
    
    #endregion
    
    #region Weapon

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isShootPressed = true;
            playerController.OnShoot();
        }

        if (context.canceled)
        {
            _isShootPressed = false;
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            playerController.OnThrowOrGrab();
        }
    }
    
    #endregion
}
