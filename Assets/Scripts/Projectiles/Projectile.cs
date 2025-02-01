using System;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    private Rigidbody2D _rigidbody;
    
    public float Speed;
    public int Damage;
    public float MaxLifeTime;
    
    [Header("Explosive")]
    public bool Explosive = false;
    public float ExplosionRange;
    
    private float _currentLifeTime;
    
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!IsServer)
        {
            return;
        }
        
        if (other.rigidbody.TryGetComponent(out HealthComponent healthComponent))
        {
            healthComponent.DamageServerRpc(Damage);
        }
        
        NetworkObject.Despawn(true);
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        
        _currentLifeTime += Time.deltaTime;
        if (_currentLifeTime >= MaxLifeTime)
        {
            NetworkObject.Despawn(true);
        }
    }
}
