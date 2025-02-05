using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimScript : NetworkBehaviour
{
    [SerializeField] private Animator _Animator;
    [SerializeField] private OwnerNetworkAnimator _OwnerNetworkAnimator;
    [SerializeField] private GameObject unArmed;
    private Dictionary<int, Animator> _Animators = new Dictionary<int, Animator>();

    private void OnConnectedToServer()
    {
        int i = 0;
        foreach (Animator animator in GetComponentsInChildren<Animator>(includeInactive:true))
        {
            _Animators.Add(i++, animator);
        }
    }
    
    [Rpc(SendTo.Everyone)]
    public void setAnimatorRpc(int AnimID)
    {
        if (!IsOwner) return;
        unArmed.SetActive(false);
        if (_Animator != null)
        {
            _Animator.gameObject.SetActive(false);
        }
        _Animator = _Animators.TryGetValue(AnimID, out Animator animator) ? animator : null;
        _Animator.gameObject.SetActive(true);
        _OwnerNetworkAnimator.Animator =_Animator;
    }

    public void RemoveAnimator()
    {
        unArmed.SetActive(true);
        _Animator.gameObject.SetActive(false);
        _Animator = null;
        _OwnerNetworkAnimator = null;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(_Animators.TryGetValue(0, out Animator animator) ? animator : null);
            setAnimatorRpc(0);
        }
        
        if (Input.GetKeyDown(KeyCode.Space)&& IsOwner)
        {
            StartAnim();
        }

        if (Input.GetKeyUp(KeyCode.Space) && IsOwner)
        {
            StopAnim();
        }
    }

    public void StartAnim()
    {
        if (_Animator == null) return;
        _Animator.SetBool("Attacking", true);
    }

    public void StopAnim()
    {
        if (_Animator == null) return;
        _Animator.SetBool("Attacking", false);
    }
}
