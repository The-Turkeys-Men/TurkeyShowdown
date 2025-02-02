using System;
using Unity.Mathematics;
using UnityEngine;

using Unity.Netcode;
using Unity.VisualScripting;


public class BaseWeapon : NetworkBehaviour, IWeapon
{
    [field:SerializeField] public float FireRate { get; set; }
    public NetworkVariable<float> FireRateTimer { get; set; } = new(0);
    [field:SerializeField] public float Damage { get; set; }
    [field:SerializeField] public int MaxAmmo { get; set; }
    [field:SerializeField] public NetworkVariable<int> Ammo { get; set; } = new(0);
    [field:SerializeField] public int SprayAmount { get; set; }
    [field:SerializeField] public float RecoilForce { get; set; }
    [field:SerializeField] public float KnockbackForce { get; set; }
    [field:SerializeField] public bool CanBeThrowed { get; set; }
    [field:SerializeField] public NetworkVariable<bool> CanBePickUp { get; set; } = new(true);
    
    [field:Header("projectile")]
    [field:SerializeField] public GameObject ProjectilePrefab { get; set; }
    [field:SerializeField] public float ProjectileSpeed { get; set; }
    [field:SerializeField] public float MaxLifetime { get; set; }
    
    [field:Header("raycast")]
    [field:SerializeField] public float MaxDistance { get; set; }
    
    public Rigidbody2D Rb ;
    public float lance ;

    public NetworkVariable<bool> IsThrowed = new(false);

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    private void Initialize()
    {
        if (!IsServer)
        {
            return;
        }
        Ammo.Value = MaxAmmo;
        CanBePickUp.Value = true;
        Rb = GetComponent<Rigidbody2D>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initialize();
    }

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
        if (!IsServer)
        {
            return;
        }
        
        if (FireRateTimer.Value > 0)
        {
            FireRateTimer.Value -= Time.deltaTime;
        }
        
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

    

    public virtual void tryShoot()
    {
        switch (Ammo.Value)
        {
            case > 0:
            {
                if(FireRateTimer.Value <= 0)
                {
                    Shoot();
                }

                break;
            }
            case 0:
                Debug.Log("Out of Ammo");
                break;
        }
    }
    
    public virtual void Shoot()
    {
        Vector2 direction = transform.right;
        SpawnProjectileServerRpc(transform.position, direction);
        OnShootServerRpc();
    }
    
    [ServerRpc]
    public virtual void OnShootServerRpc()
    {
        FireRateTimer.Value = FireRate;
        Ammo.Value -= 1;
    }

    [ServerRpc]
    public void SpawnProjectileServerRpc(Vector3 position, Vector2 direction)
    {
        direction.Normalize();
        GameObject spawnedBullet = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spawnedBullet.GetComponent<NetworkObject>().Spawn();
        spawnedBullet.GetComponent<Projectile>().Direction = direction;
    }
}
