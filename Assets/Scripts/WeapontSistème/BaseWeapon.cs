using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;
using WeaponSystem;


public class BaseWeapon : NetworkBehaviour, IWeapon
{
    [field:SerializeField] public float FireRate { get; set; }
    public NetworkVariable<float> FireRateTimer { get; set; } = new(0);
    [field:SerializeField] public int Damage { get; set; }
    [field:SerializeField] public int MaxAmmo { get; set; }
    [field:SerializeField] public NetworkVariable<int> Ammo { get; set; } = new(0);
    [field:SerializeField] public int SprayAmount { get; set; }
    [field:SerializeField] public float RecoilForce { get; set; }
    [field:SerializeField] public float KnockbackForce { get; set; }
    [field:SerializeField] public bool CanBeThrowed { get; set; }
    [field:SerializeField] public NetworkVariable<bool> CanBePickUp { get; set; } = new(true);
    [field:SerializeField] public ShootType WeaponShootType { get; set;  }

    [field:Header("throw")]
    [field:SerializeField] public int DamageByThrow { get; set; }
    [field:SerializeField] public float ThrowForce { get; set; }
    [field:SerializeField] public float ThrowTorque { get; set; }
    [field:SerializeField] public float ThrowKnockbackForce { get; set; }

    [field:Header("projectile")]
    [field:SerializeField] public GameObject ProjectilePrefab { get; set; }
    [field:SerializeField] public float ProjectileSpeed { get; set; }
    [field:SerializeField] public float MaxLifetime { get; set; }
    
    [field:Header("raycast")]
    [field:SerializeField] public float MaxDistance { get; set; }

    public Transform ShootPoint { get; set; }

    [Header("Components")]
    public Rigidbody2D Rb ;
    public GameObject Visuals;

    public NetworkVariable<bool> IsThrowed = new(false);

    public UnityEvent OnGrab { get; set; } = new();
    
    public GameObject LastOwner { get; set; }

    private bool _isDespawning = false;
    
    private float _fireRateTimer;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        FireRateTimer.OnValueChanged += (previous, current) =>
        {
            _fireRateTimer = current;
        };
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || _isDespawning)
        {
            return;
        }
        
        if (IsThrowed.Value) 
        { 
            _isDespawning = true;
            GetComponent<NetworkObject>().Despawn(true);
            if (other.TryGetComponent(out HealthComponent healthComponent))
            {
                healthComponent.DamageServerRpc(DamageByThrow, LastOwner.GetComponent<NetworkObject>().NetworkObjectId);
            }
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
            CanBePickUp.Value = true;
        }
    }

    

    public virtual void tryShoot()
    {
        switch (Ammo.Value)
        {
            case > 0:
            {
                if(_fireRateTimer <= 0)
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
        switch (WeaponShootType)
        {
            case ShootType.Projectile:
                SpawnProjectileServerRpc(ShootPoint.position, direction);
                break;
            case ShootType.Raycast:
                RaycastHit2D raycastResult = Physics2D.Raycast(ShootPoint.position, direction, MaxDistance,
                    (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("World")));
                
                if (raycastResult && raycastResult.collider.TryGetComponent(out HealthComponent healthComponent))
                {
                    healthComponent.DamageServerRpc(Damage, NetworkObjectId);
                }
                
                Traine(raycastResult,direction);
                
                break;
        }
        _fireRateTimer = FireRate;
        OnShootServerRpc();
        Rigidbody2D playerRigidbody = transform.parent.GetComponentInParent<Rigidbody2D>();
        playerRigidbody.AddForce(-direction * RecoilForce, ForceMode2D.Impulse);
    }
    void Traine(RaycastHit2D raycastResult,Vector2 direction )
    {
        Vector2 endPoint;
        if(raycastResult==true)
        {
            endPoint = raycastResult.point;
        }
        else
        {
            endPoint = direction*MaxDistance;
        }

        GameObject tempTrainé=new GameObject("tempTrainé");
        DespawnTraine despawnTraine = tempTrainé.AddComponent<DespawnTraine>();
        tempTrainé.transform.position=Vector3.zero;
        LineRenderer lineRenderer = tempTrainé.AddComponent<LineRenderer>();
        lineRenderer.material.color=Color.black;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
        
    }
    
    
    
        
        
        
    
            
    
    
    [ServerRpc]
    protected virtual void OnShootServerRpc()
    {
        FireRateTimer.Value = FireRate;
        Ammo.Value -= 1;
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector2 position, Vector2 direction)
    {
        direction.Normalize();
        GameObject spawnedBullet = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spawnedBullet.GetComponent<NetworkObject>().Spawn();
        spawnedBullet.GetComponent<Projectile>().Direction = direction;
        
    }


    [ServerRpc]
    public void HideServerRpc()
    {
        HideClientRpc();
    }
    
    [ClientRpc]
    private void HideClientRpc()
    {
        Visuals.SetActive(false);
    }
    
    [ServerRpc]
    public void ShowServerRpc()
    {
        ShowClientRpc();
    }
    
    [ClientRpc]
    private void ShowClientRpc()
    {
        Visuals.SetActive(true);
    }
}
