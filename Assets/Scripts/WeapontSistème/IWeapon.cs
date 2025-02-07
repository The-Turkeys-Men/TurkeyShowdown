using Unity.Netcode;
using UnityEngine;
using WeaponSystem;

public interface IWeapon : IGrabbable
{
    public int WeaponId{get;}
    
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
    public bool IsExplosive { get; }
    public int ExplosionDamage { get; }
    public float ExplosionRange { get; }
    public float ExplosionSelfKnockback { get; }
    public float ExplosionKnockback { get; }
    //[Header("raycast")]
    public float MaxDistance{get;} 
    
    public Transform ShootPoint { get; }
    //[Header("Melee")]
    public Vector2 MeleeRange{get;}
    public float WallHitBoost{get;}

    public void tryShoot();//verification
    public void Shoot();


    
    

}


