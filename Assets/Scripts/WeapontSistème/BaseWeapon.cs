using Extensions;
using Projectiles;
using UnityEngine;

using Unity.Netcode;
using UnityEngine.Events;
using WeaponSystem;


public class BaseWeapon : NetworkBehaviour, IWeapon
{
    [field:SerializeField] public int WeaponId { get; set; }
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
    [field:SerializeField] public bool IsExplosive { get; set; }
    [field:SerializeField] public int ExplosionDamage { get; set; }
    [field:SerializeField] public float ExplosionRange { get; set; }
    [field:SerializeField] public float ExplosionSelfKnockback { get; set; }
    [field:SerializeField] public float ExplosionKnockback { get; set; }

    [field:Header("raycast")]
    [field:SerializeField] public float MaxDistance { get; set; }

    [SerializeField] public Color RaycastColor;

    [field:Header("Melee")]
    [field:SerializeField] public Vector2 MeleeRange { get; set; }
    [field:SerializeField] public float WallHitBoost { get; set; }
    
    [Header("Trainée feedback")]
    public Material TrailMaterial;
    
    [Header("Components")]
    public Rigidbody2D Rb ;
    public GameObject Visuals;
    public Transform ShootPoint { get; set; }

    public NetworkVariable<bool> IsThrowed = new(false);
    public NetworkVariable<bool> ShouldHide = new(false);

    public UnityEvent OnGrab { get; set; } = new();
    
    public GameObject LastOwner { get; set; }

    private bool _isDespawning = false;
    private float _throwSpeedThreshold = 0.2f;

    [SerializeField] private string _nomTir;
    [SerializeField] private string _nomLancer;
    [SerializeField] private string _nomColition;
     [SerializeField] private string _nomHit;

     [SerializeField] private float baseVolume;



    
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

        if (other.isTrigger)
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
            AudioManager.Instance.PlaySFX(_nomColition,transform.position,baseVolume);
            if (other.TryGetComponent(out HealthComponent healthComponent))
            {
                healthComponent.Damage(DamageByThrow, LastOwner.GetComponent<NetworkObject>().NetworkObjectId);
            }

            if (other.attachedRigidbody)
            {
                other.attachedRigidbody.AddForce(Rb.linearVelocity.normalized * KnockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    
    void Update()
    {
        Visuals.SetActive(!ShouldHide.Value);
        
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
        Rigidbody2D playerRigidbody = transform.parent.GetComponentInParent<Rigidbody2D>();
        Vector2 direction = transform.right;
        var teamComponent = LastOwner.GetComponent<TeamComponent>();
        int teamIDValue = (teamComponent)? teamComponent.TeamID.Value : -1;
        switch (WeaponShootType)
        {
            case ShootType.Projectile:
                SpawnProjectileServerRpc(ShootPoint.position, direction);
                break;
            case ShootType.Raycast:
                
                RaycastHit2D raycastResult = RaycastUtils.RaycastFirstEnnemy(teamIDValue, ShootPoint.position, direction, MaxDistance, 
                    (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("World")));
                
                if (raycastResult && raycastResult.collider.TryGetComponent(out HealthComponent healthComponent))
                {
                    healthComponent.DamageServerRpc(Damage, LastOwner.GetNetworkObjectId());
                }
                
                if (raycastResult && raycastResult.collider.attachedRigidbody)
                {
                    if (raycastResult.collider.attachedRigidbody.TryGetComponent(out KnockbackHandler knockbackHandler))
                    {
                        knockbackHandler.ApplyKnockbackServerRpc(direction, KnockbackForce);
                    }
                }
                
                Vector2 endPoint;
                if(raycastResult)
                {
                    endPoint = raycastResult.point;
                }
                else
                {
                    endPoint = (Vector2)ShootPoint.position + direction * MaxDistance;
                }
                SpawnBulletTrailServerRpc(endPoint);
                
                break;
            case ShootType.Melee:
                var overlapResult = Physics2D.OverlapBoxAll(ShootPoint.position, MeleeRange, ShootPoint.eulerAngles.z, 
                    (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("World")));

                bool appliedWallboost = false;
                foreach (Collider2D collider in overlapResult)
                {
                    if (collider.isTrigger)
                    {
                        continue;
                    }
                    
                    if (collider.gameObject.layer == LayerMask.NameToLayer("World") && !appliedWallboost)
                    {
                        playerRigidbody.AddForce(-direction * WallHitBoost, ForceMode2D.Impulse);
                        appliedWallboost = true;

                        continue;
                    }
                    
                    if (collider.TryGetComponent(out TeamComponent otherTeamComponent) && teamIDValue == otherTeamComponent.TeamID.Value)
                    {
                        continue;
                    }

                    if (!collider.TryGetComponent(out HealthComponent healthComponent2))
                    {
                        continue;
                    }
                    
                    healthComponent2.DamageServerRpc(Damage, LastOwner.GetNetworkObjectId());
                    AudioManager.Instance.PlaySFX(_nomHit,transform.position,baseVolume);
                    if (collider.attachedRigidbody)
                    {
                        if (collider.attachedRigidbody.TryGetComponent(out KnockbackHandler knockbackHandler))
                        {
                            knockbackHandler.ApplyKnockbackServerRpc(direction, KnockbackForce);
                        }
                    }
                }
                
                break;
        }
        AudioManager.Instance.PlaySFX(_nomTir,transform.position,baseVolume);
        FireRateTimer = FireRate;
        OnShootServerRpc();
        playerRigidbody.AddForce(-direction * RecoilForce, ForceMode2D.Impulse);
        
        LastOwner.GetComponent<AnimScript>().StartAnim();
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
        despawnTraine.StartWidth = 0.15f;
        LineRenderer lineRenderer = tempTrainé.AddComponent<LineRenderer>();
        lineRenderer.material = new(TrailMaterial);
        lineRenderer.material.color = RaycastColor;
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
        spawnedBullet.GetComponent<NetworkObject>().Spawn(true);
        var projectile = spawnedBullet.GetComponent<BaseProjectile>();
        projectile.Direction = direction;
        projectile.SenderObject = LastOwner;
        
        projectile.IsExplosive = IsExplosive;
        projectile.ExplosionSelfKnockback = ExplosionSelfKnockback;
        projectile.ExplosionDamage = ExplosionDamage;
        projectile.ExplosionRange = ExplosionRange;
        projectile.ExplosionKnockback = ExplosionKnockback;
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void HideClientRpc()
    {
        Visuals.SetActive(false);
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ShowClientRpc()
    {
        Visuals.SetActive(true);
    }
}
