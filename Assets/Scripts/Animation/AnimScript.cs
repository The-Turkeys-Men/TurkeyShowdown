using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class AnimScript : NetworkBehaviour
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private SpriteRenderer _Arm;
    //[SerializeField] private Sprite _NoArm;

    [Rpc(SendTo.Server)]
    public void SetAnimatorServerRpc(int animId)
    {
        SetAnimatorClientRpc(animId);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void SetAnimatorClientRpc(int animId)
    {
        setAnimator(animId);
    }
    
    public void setAnimator(int AnimID)
    {
        _Animator.SetInteger("index", AnimID);
    }

    [Rpc(SendTo.Server)]
    public void RemoveAnimatorServerRpc()
    {
        RemoveAnimatorClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void RemoveAnimatorClientRpc()
    {
        RemoveAnimator();
    }

    public void RemoveAnimator()
    {
        _Animator.SetInteger("index", -1);
        //_Arm.sprite = _NoArm;
    }
    
    public void StartAnim()
    {
        if (_Animator == null) return;
        _Animator.SetTrigger("Attacking");
    }
}
