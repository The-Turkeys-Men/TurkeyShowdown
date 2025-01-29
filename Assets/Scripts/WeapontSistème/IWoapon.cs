using UnityEngine;

public interface IWoapon 
{
    
    public float FireRate{get;}//cadance 
    public float Damage{get;}
    public int MaxAmmo{get;}// munition max
    public int SprayAmount{get;}
    public float RecoilForce{get;}// soi meme 
    public float KnockbacForec{get;}
    public bool CanDeThrowed{get;}
    public float FireRilode{get;}
    //[Header("projectile")]
    public GameObject ProjectilePrefab{get;}
    public float ProjectileSpeed{get;}
    public float MaxLiftime{get;}
    //[Header("raycast")]
    public float MaxDistance{get;} 
    

    public void tryShoot();//verification
    public void TryThrowWeapon();
    protected void Shoot();
    protected void ThrowWeapon();


    
    

}


