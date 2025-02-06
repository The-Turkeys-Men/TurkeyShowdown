using System;
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
    public bool Explosive = false;
    public float ExplosionRange;

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
            NetworkObject.Despawn(true);
        }
    }
}
