using System;
using Unity.Netcode;
using UnityEngine;

namespace Health
{
    public class PlayerDeathComponent : NetworkBehaviour
    {
        private void Awake()
        {
            var healthComponent = GetComponent<HealthComponent>();
            healthComponent.OnDeath.AddListener(OnDeath);
            healthComponent.OnRespawn.AddListener(OnRespawn);
        }

        private void OnRespawn()
        {
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = true;
            
            var colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = true;
            }
        }

        private void OnDeath(ulong arg0)
        {
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = false;
            
            var colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
}