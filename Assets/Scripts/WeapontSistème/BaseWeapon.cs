using Debugger;
using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;
using WeaponSystem;


public class BaseWeapon : NetworkBehaviour, IWeapon
{
    [field:SerializeField] public float FireRate { get; set; }
    public float FireRateTimer { get; set; }
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
    
    [Header("Trainée feedback")]
    public Material TrailMaterial;
    
    [Header("Components")]
    public Rigidbody2D Rb ;
    public GameObject Visuals;

    public NetworkVariable<bool> IsThrowed = new(false);

    public UnityEvent OnGrab { get; set; } = new();
    
    public GameObject LastOwner { get; set; }

    private bool _isDespawning = false;
    private float _throwSpeedThreshold = 0.2f;
    
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

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer || _isDespawning)
        {
            return;
        }

        if (!IsThrowed.Value)
        {
            return;
        }

        LastOwner.TryGetComponent(out TeamComponent lastOwnerTeam);
        
        if (!other.TryGetComponent(out TeamComponent otherTeam) || otherTeam.TeamID.Value != lastOwnerTeam.TeamID.Value)
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
        if (FireRateTimer > 0)
        {
            FireRateTimer -= Time.deltaTime;
        }
        
        if (!IsServer)
        {
            return;
        }
        
        if (!IsThrowed.Value)
        {
            return;
        }

        if (Rb.linearVelocity.magnitude > _throwSpeedThreshold)
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
                if(FireRateTimer <= 0)
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
                var teamComponent = LastOwner.GetComponent<TeamComponent>();
                int teamIDValue = (teamComponent)? teamComponent.TeamID.Value : -1;
                
                RaycastHit2D raycastResult = RaycastUtils.RaycastFirstEnnemy(teamIDValue, ShootPoint.position, direction, MaxDistance, 
                    (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("World")));
                
                if (raycastResult && raycastResult.collider.TryGetComponent(out HealthComponent healthComponent))
                {
                    healthComponent.DamageServerRpc(Damage, NetworkObjectId);
                }
                
                Vector2 endPoint;
                if(raycastResult==true)
                {
                    endPoint = raycastResult.point;
                }
                else
                {
                    endPoint = (Vector2)ShootPoint.position + direction * MaxDistance;
                }
                SpawnBulletTrailServerRpc(endPoint);
                
                break;
        }
        FireRateTimer = FireRate;
        OnShootServerRpc();
        Rigidbody2D playerRigidbody = transform.parent.GetComponentInParent<Rigidbody2D>();
        playerRigidbody.AddForce(-direction * RecoilForce, ForceMode2D.Impulse);
    }
    
    [Rpc(SendTo.Server)]
    private void SpawnBulletTrailServerRpc(Vector2 endPoint)
    {
        SpawnBulletTrailClientRpc(endPoint);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnBulletTrailClientRpc(Vector2 endPoint)
    {
        GameObject tempTrainé=new GameObject("tempTrainé");
        DespawnTraine despawnTraine = tempTrainé.AddComponent<DespawnTraine>();
        tempTrainé.transform.position=Vector3.zero;
        LineRenderer lineRenderer = tempTrainé.AddComponent<LineRenderer>();
        lineRenderer.material = new(TrailMaterial);
        lineRenderer.material.color=Color.black;
        lineRenderer.SetPosition(0, ShootPoint.position);
        lineRenderer.SetPosition(1, endPoint);
    }
    
    [Rpc(SendTo.Server)]
    protected virtual void OnShootServerRpc()
    {
        FireRateTimer = FireRate;
        Ammo.Value -= 1;
    }

    [Rpc(SendTo.Server)]
    private void SpawnProjectileServerRpc(Vector2 position, Vector2 direction)
    {
        direction.Normalize();
        GameObject spawnedBullet = Instantiate(ProjectilePrefab, position, Quaternion.identity);
        spawnedBullet.GetComponent<NetworkObject>().Spawn();
        spawnedBullet.GetComponent<Projectile>().Direction = direction;
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void HideServerRpc()
    {
        DebuggerConsole.Instance.LogServerRpc("hide weapon on server");
        HideClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void HideClientRpc()
    {
        Visuals.SetActive(false);
        DebuggerConsole.Instance.Log("hide weapon on client");
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void ShowServerRpc()
    {
        ShowClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ShowClientRpc()
    {
        Visuals.SetActive(true);
        DebuggerConsole.Instance.Log("hide weapon on client");
    }
}
