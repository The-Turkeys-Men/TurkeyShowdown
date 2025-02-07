using System;
using Debugger;
using Unity.Netcode;
using UnityEngine;

namespace Health
{
    public class PlayerDeathComponent : NetworkBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        
        private LayerMask _originalExcludeLayers;
        
        public Action OnDeathEvent;
        public Action OnRespawnEvent;
        
        private void Awake()
        {
            var healthComponent = GetComponent<HealthComponent>();
            healthComponent.OnDeath.AddListener(OnDeath);
            healthComponent.OnRespawn.AddListener(OnRespawn);
            
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _originalExcludeLayers = _rigidbody2D.excludeLayers;
        }

        private void OnRespawn()
        {
            OnRespawnEvent?.Invoke();
            
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = true;
            
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.excludeLayers = _originalExcludeLayers;
            
        }

        private void OnDeath(ulong arg0)
        {
            OnDeathEvent?.Invoke();
            
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = false;

            _rigidbody2D.excludeLayers = ~(1 << LayerMask.NameToLayer("World"));

            if (!IsOwner)
            {
                return;
            }
            
            GetComponent<Grappler>().TryReleaseGrab();
            PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();
            if (playerWeapon.EquipedWeapon && playerWeapon.EquipedWeapon.CanBeThrowed)
            {
                playerWeapon.ThrowWeapon();
            }
        }
    }
}