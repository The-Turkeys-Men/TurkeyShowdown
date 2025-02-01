using UnityEngine;

using Unity.Netcode;
using Unity.VisualScripting;


public class BaseWeapon : NetworkBehaviour, IWeapon
{
    public float FireRate { get; set; }
    public float Damage { get; set; }
    public int MaxAmmo { get; set; }
    public int SprayAmount { get; set; }
    public float RecoilForce { get; set; }
    public float KnockbackForce { get; set; }
    public bool CanBeThrowed { get; set; }
    public float FireReload { get; set; }
    public NetworkVariable<int> Ammo { get; set; }
    public bool PickUp { get; set; }
    public GameObject ProjectilePrefab { get; set; }
    public float ProjectileSpeed { get; set; }
    public float MaxLifetime { get; set; }
    public float MaxDistance { get; set; }
    
    public Rigidbody2D Rb ;
    public float lance ;

    public NetworkVariable<bool> IsThrowed;
    
     public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
        {
            return;
        }
        
        if(IsThrowed.Value) 
        { 
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    
    void Update()
    {
        if (!IsThrowed.Value)
        {
            return;
        }

        if (!Mathf.Approximately(Rb.linearVelocity.magnitude, 0))
        {
            return;
        }
        
        if (Ammo.Value == 0)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
        else
        {
            IsThrowed.Value = false;
        }
    }
    public void ThrowWeapon()
    {
       Rb.linearVelocity=transform.TransformDirection(Vector3.forward*lance);
    }

    

    public void tryShoot()
    {
        switch (Ammo.Value)
        {
            case > 0:
            {
                if((FireReload+=Time.deltaTime)>=FireRate)
                {
                    Shoot();
                    FireReload-=FireReload;
                }

                break;
            }
            case 0:
                ThrowWeapon();
                break;
        }
    }
    
    public void  Shoot()
    {
        Instantiate(this.ProjectilePrefab,this.transform.position,this.transform.rotation);
        
        Ammo.Value -= 1;
    }

    public void TryThrowWeapon()
    {
        if(CanBeThrowed==false)
        {
            return;
        }
        ThrowWeapon();
    }
}
