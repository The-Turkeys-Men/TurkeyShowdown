using UnityEngine;

using Unity.Netcode;
using Unity.VisualScripting;


public class BaseWoapon : NetworkBehaviour 
{
    public float FireRate ;

    public float Damage  ;

    public int MaxAmmo ;
    public int SprayAmount ;

    public float RecoilForce ;

    public float KnockbacForec ;

    public bool CanDeThrowed ;

    public float FireRilode ;

    public int Ammo ;

    public GameObject ProjectilePrefab ;
    public bool PickUp ;
    

    public float ProjectileSpeed ; 
    public float MaxLiftime ;
    public float MaxDistance ;
    public Rigidbody2D Rb ;
    public float lance ;
    public bool Equipped=false;
    
     public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!Mathf.Approximately(Rb.linearVelocity.magnitude, 0))
            {
                Destroy(this);

            }

    }

    
    void Update()
    {
        
        TryThrowWeapon();

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
    public void ThrowWeapon()
    {
       Equipped=false;
      
        Rb.linearVelocity=transform.TransformDirection(Vector3.forward*lance);

    }

    public void tryShoot()
    {
        
        
        if(Equipped==true)
        {
            if(Ammo>0)
            {
                if((FireRilode+=Time.deltaTime)>=FireRate)
                {
                    Shoot();
                    FireRilode-=FireRilode;
                }

            }
            else 
            if (Ammo==0)
            {
                Debug.Log("truc2");
                ThrowWeapon();
            }
        }

    }
    protected void truc()
    {
        Shoot();
    }
    public void  Shoot()
    {
        
        Instantiate(this.ProjectilePrefab,this.transform.position,this.transform.rotation);
        
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
