using Unity.Netcode;
using UnityEngine;

public class AnimScript : NetworkBehaviour
{
    [SerializeField] private Animator _Animator;

    private void Update()
    {
        if (IsOwner) 
            AnimAttacking();
    }

    public void ChangeWeapon(Animator animator)
    {
        _Animator = animator;
    }
    
    
    public void AnimAttacking()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _Animator.SetBool("IsAttacking", true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _Animator.SetBool("IsAttacking", false);
        }
    }
}
