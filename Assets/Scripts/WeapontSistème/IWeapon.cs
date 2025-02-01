using Unity.Netcode;
using UnityEngine;

public interface IWeapon 
{
    
    public float FireRate{get;}//cadance 
    public float Damage{get;}
    public int MaxAmmo{get;}// munition max
    public int SprayAmount{get;}
    public float RecoilForce{get;}// soi meme 
    public float KnockbackForce{get;}
    public bool CanBeThrowed{get;}
    public float FireReload{get;}
    public NetworkVariable<int> Ammo {get;}
    public bool PickUp{get;}
    //[Header("projectile")]
    public GameObject ProjectilePrefab{get;}
    public float ProjectileSpeed{get;}
    public float MaxLifetime{get;}
    //[Header("raycast")]
    public float MaxDistance{get;} 
    

    public void tryShoot();//verification
    public void TryThrowWeapon();
    public void Shoot();
    public void ThrowWeapon();


    
    

}


