using Unity.Netcode;
using UnityEngine;
using WeaponSystem;

public interface IWeapon : IGrabbable
{
    
    public float FireRate{get;}//cadance 
    public float FireRateTimer{get;}
    public int Damage{get;}
    public int MaxAmmo{get;}// munition max
    public int SprayAmount{get;}
    public float RecoilForce{get;}// soi meme 
    public float KnockbackForce{get;}
    public bool CanBeThrowed{get;}
    public NetworkVariable<int> Ammo {get;}
    public NetworkVariable<bool> CanBePickUp{get;}
    public ShootType WeaponShootType { get; }
    //[Header("throw")]
    public int DamageByThrow{get;}
    public float ThrowForce{get;}
    public float ThrowTorque{get;}
    public float ThrowKnockbackForce{get;}
    
    //[Header("projectile")]
    public GameObject ProjectilePrefab{get;}
    public float ProjectileSpeed{get;}
    public float MaxLifetime{get;}
    //[Header("raycast")]
    public float MaxDistance{get;} 
    
    public Transform ShootPoint { get; }

    public void tryShoot();//verification
    public void Shoot();


    
    

}


