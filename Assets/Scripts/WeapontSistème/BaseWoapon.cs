using UnityEngine;

using Unity.Netcode;
using Unity.VisualScripting;


public class BaseWoapon : NetworkBehaviour,IWoapon

{
    public float FireRate {get;}

    public float Damage  {get;}

    public int MaxAmmo {get;}
    public int SprayAmount {get;set;}

    public float RecoilForce {get;}

    public float KnockbacForec {get;}

    public bool CanDeThrowed {get;}

    public float FireRilode {get;set;}

    public int Ammo {get;set;}

    public GameObject ProjectilePrefab {get;}
    public bool PickUp {get; set;}
    

    public float ProjectileSpeed {get;}
    public float MaxLiftime {get;}
    public float MaxDistance {get;}
    public Rigidbody2D Rb;
    public float lance;
    public bool Equipped;
    


    
     public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!Mathf.Approximately(Rb.linearVelocity.magnitude, 0))
            {
                Destroy(this);
            }

    }

    
    void Update()
    {
        
        if(Mathf.Approximately(Rb.linearVelocity.magnitude, 0))
            {
                
                if(Ammo>0)
                {
                   PickUp=true; 
                }
                else if (Ammo==0)
                {
                    Destroy(this);
                    
                }
            }
    }
    protected void Test()
    {
        ThrowWeapon(); 
    }
    public void ThrowWeapon()
    {
       Equipped=false;
        transform.SetParent(null);
        Rb.linearVelocity=transform.TransformDirection(Vector3.forward*lance);

    }

    public void tryShoot()
    {
        
        
            if(Ammo>0)
            {
                if((FireRilode+=Time.deltaTime)>=FireRate)
                {
                    Shoot();
                    FireRilode-=FireRilode;
                }

            }
            else if (Ammo==0)
            {
                ThrowWeapon();
            }
            

        

    }
    public void Shoot()
    {
        
        Instantiate(ProjectilePrefab,transform.position,transform.rotation);
        
        Ammo-=1;
    }

    public void TryThrowWeapon()
    {
        if(CanDeThrowed==false)
        {
            return;
        }
        
        
            
            ThrowWeapon();
            
            
        
    }
    
}
