using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Projectiles
{
    public class BaseProjectile : NetworkBehaviour
    {
        [HideInInspector] public GameObject SenderObject;
        protected Rigidbody2D _rigidbody;
    
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
    
        protected float _currentLifeTime;
        [HideInInspector] public Vector2 Direction;
        [SerializeField] protected float _baseVolume;
        
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
    }
}