using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Network
{
    public class NetworkTransformCustom : NetworkBehaviour
    {
        public float interpolationSpeed = 10f;
        private Vector3 targetPosition;

        void Update()
        {
            if (IsServer)
            {
                UpdatePositionClientRpc(transform.position);
                return;
            }
        
            if (!IsOwner)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * interpolationSpeed);
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer || IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SetTargetPosition;
            }
        }

        private void SetTargetPosition(ulong clientId)
        {
            targetPosition = transform.position;
        }

        [ClientRpc]
        private void UpdatePositionClientRpc(Vector3 newPosition)
        {
            targetPosition = newPosition;
        }

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback -= SetTargetPosition;
        }
    }
}