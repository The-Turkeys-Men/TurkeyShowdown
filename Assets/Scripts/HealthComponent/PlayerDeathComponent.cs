using System;
using System.Collections;
using Debugger;
using Unity.Netcode;
using UnityEngine;

namespace Health
{
    public class PlayerDeathComponent : NetworkBehaviour
    {
        [SerializeField] private RespawnButton respawnButton;
        private Rigidbody2D _rigidbody2D;
        
        private LayerMask _originalExcludeLayers;
        
        public Action OnDeathEvent;
        public Action OnRespawnEvent;
        
        [SerializeField] private GameObject _deathSpriteRenderer;
        [SerializeField] private GameObject _aliveVisuals;
        
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
            
            respawnButton.enabled = false;
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = true;
            
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.excludeLayers = _originalExcludeLayers;
            
            ShowAliveVisuals();
            ShowAliveVisualsServerRpc();
        }

        private void OnDeath(ulong killer)
        {
            OnDeathEvent?.Invoke();
            if (IsOwner)
            {
                respawnButton.enabled = true;
            }
            
            var killerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[killer].gameObject;
            
            var playerController = GetComponent<PlayerController>();
            playerController.InputActivated = false;

            _rigidbody2D.excludeLayers = ~(1 << LayerMask.NameToLayer("World"));

            if (!IsOwner)
            {
                return;
            }
            
            KillFeedManager.Instance.AddKillServerRpc(killerObject.name, gameObject.name, 0);
            DebuggerConsole.Instance.LogServerRpc(killerObject.name + " addkill");
            
            GetComponent<Grappler>().TryReleaseGrab();
            PlayerWeapon playerWeapon = GetComponent<PlayerWeapon>();
            if (playerWeapon.EquipedWeapon && playerWeapon.EquipedWeapon.CanBeThrowed)
            {
                playerWeapon.ThrowWeapon();
            }
            
            ShowDeathVisuals();
            ShowDeathVisualsServerRpc();
        }
        
        [Rpc(SendTo.Server)]
        private void ShowDeathVisualsServerRpc()
        {
            ShowDeathVisualsClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ShowDeathVisualsClientRpc()
        {
            if (IsOwner)
            {
                return;
            }
            ShowDeathVisuals();
        }

        private void ShowDeathVisuals()
        {
            _deathSpriteRenderer.SetActive(true);
            _aliveVisuals.SetActive(false);
        }

        [Rpc(SendTo.Server)]
        private void ShowAliveVisualsServerRpc()
        {
            ShowAliveVisualsClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ShowAliveVisualsClientRpc()
        {
            if (IsOwner)
            {
                return;
            }
            ShowAliveVisuals();
        }

        private void ShowAliveVisuals()
        {
            _deathSpriteRenderer.SetActive(false);
            _aliveVisuals.SetActive(true);
        }
    }
}