using System;
using Debugger;
using Extensions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : NetworkBehaviour
{
    [HideInInspector] public GameObject SenderObject;
    private Rigidbody2D _rigidbody;
    
    public float Speed;
    public int Damage;
    public float MaxLifeTime;
    
    [Header("Explosive")]
    public bool IsExplosive = false;

    public int ExplosionDamage;
    public float ExplosionRange;
    public float ExplosionKnockback;
    public float ExplosionSelfKnockback;

    public GameObject HitEffectPrefab;
    
    private float _currentLifeTime;
    [HideInInspector] public Vector2 Direction;
    
    private void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            return;
        }
        Initialize();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer)
        {
            return;
        }

        if (SenderObject.TryGetComponent(out TeamComponent senderTeamComponent) && other.TryGetComponent(out TeamComponent otherTeamComponent))
        {
            if (senderTeamComponent.TeamID == otherTeamComponent.TeamID)
            {
                return;
            }
        }
        
        if (other.transform.TryGetComponent(out HealthComponent healthComponent))
        {
            healthComponent.DamageServerRpc(Damage, SenderObject.GetNetworkObjectId());
        }
        
        if (IsExplosive)
        {
            Explode();
        }
        SpawnHitEffectRpc(transform.position);
        NetworkObject.Despawn(true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnHitEffectRpc(Vector2 position)
    {
        GameObject hitEffect = Instantiate(HitEffectPrefab, position, Quaternion.identity);
    }
    
    
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        
        _rigidbody.linearVelocity = Direction * Speed;
        
        _currentLifeTime += Time.deltaTime;
        if (_currentLifeTime >= MaxLifeTime)
        {
            if (IsExplosive)
            {
                Explode();
                SpawnHitEffectRpc(transform.position);
            }
            NetworkObject.Despawn(true);
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, ExplosionRange);
        foreach (Collider2D collider in colliders)
        {
            if (Physics2D.Linecast(transform.position, collider.transform.position,
                    1 << LayerMask.NameToLayer("World")))
            {
                continue;
            }
            
            if (SenderObject.TryGetComponent(out TeamComponent senderTeamComponent) && collider.TryGetComponent(out TeamComponent otherTeamComponent))
            {
                if (senderTeamComponent.TeamID == otherTeamComponent.TeamID)
                {
                    Vector2 direction = (collider.transform.position - transform.position).normalized;
                    var objectId = collider.gameObject.GetNetworkObjectId();
                    if (objectId != ulong.MaxValue)
                    {
                        ApplyKnockbackRpc(objectId, direction * ExplosionSelfKnockback);
                    }
                    continue;
                }
            }
            
            if (collider.TryGetComponent(out HealthComponent healthComponent))
            {
                healthComponent.Damage(ExplosionDamage, SenderObject.GetNetworkObjectId());
            }

            if (collider.TryGetComponent(out Rigidbody2D rb))
            {
                Vector2 direction = (collider.transform.position - transform.position).normalized;
                var objectId = collider.gameObject.GetNetworkObjectId();
                if (objectId != ulong.MaxValue)
                {
                    ApplyKnockbackRpc(objectId, direction * ExplosionKnockback);
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ApplyKnockbackRpc(ulong playerObjectId, Vector2 knockback)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerObjectId, out var playerObject);
        playerObject.GetComponent<Rigidbody2D>().AddForce(knockback * ExplosionSelfKnockback, ForceMode2D.Impulse);
    }
}
