using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

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
        playerController.OnShoot();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        playerController.OnThrow();
    }
    
    #endregion
}
