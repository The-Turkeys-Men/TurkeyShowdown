using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    #region Movement
    
    public void OnMove(Vector2 direction)
    {
        //movement
    }
    
    #endregion

    #region Grapple
    
    public void OnGrapple()
    {
        //TryGrapple
    }

    public void OnUnGrapple()
    {
        //TryUngrapple
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
