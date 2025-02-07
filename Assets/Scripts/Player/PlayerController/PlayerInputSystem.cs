using System;
using Debugger;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputSystem : NetworkBehaviour
{
    [FormerlySerializedAs("playerController")] [SerializeField] private PlayerController _playerController;
    [SerializeField] private InputActionAsset _inputActionAsset;
    private bool _isShootPressed;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner)
        {
            return;
        }

        var playerInput = gameObject.AddComponent<PlayerInput>();
        playerInput.actions = _inputActionAsset;
        
        var playerActionMap = _inputActionAsset.FindActionMap("Player");
        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        playerInput.onActionTriggered += ctx =>
        {
            //DebuggerConsole.Instance.Log(ctx.action.name);
            switch (ctx.action.name)
            {
                case "Move":
                    OnMove(ctx);
                    break;
                case "GrapplingHook":
                    OnGrapple(ctx);
                    break;
                case "Attack":
                    OnShoot(ctx);
                    break;
                case "Throw":
                    OnThrow(ctx);
                    break;
            }
        };

    }


    private void Update()
    {
        if (_isShootPressed)
        {
            _playerController.OnShoot();
        }
    }

    #region Movement

   public  void OnMove(InputAction.CallbackContext context)
    {
        _playerController.OnMove(context.ReadValue<Vector2>());
    }

    #endregion
    
    #region Grapple
    
    public void OnGrapple(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _playerController.OnGrapple();
        }
        if (context.canceled)
        {
            _playerController.OnUnGrapple();
        }
    }
    
    #endregion
    
    #region Weapon

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _isShootPressed = true;
            _playerController.OnShoot();
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
            _playerController.OnThrowOrGrab();
        }
    }
    
    #endregion
}
