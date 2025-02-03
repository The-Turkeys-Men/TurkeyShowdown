using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network
{
    public class RelayManager : MonoBehaviour
    {
        public async Task<string> CreateRelay(int maxPlayers)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);

                // Récupérer les données nécessaires pour configurer le transport
                RelayServerData relayServerData = new RelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.ConnectionData,
                    allocation.ConnectionData,
                    allocation.Key,
                    false, // Désactiver DTLS
                    true   // Activer WebSockets (WSS)
                );

                // Configurer Unity Transport
                UnityTransport transport = FindObjectOfType<UnityTransport>();
                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log("Join Code: " + joinCode);
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        

        public async Task JoinRelay(string joinCode)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                // Récupérer les données nécessaires pour configurer le transport
                RelayServerData relayServerData = new RelayServerData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData,
                    joinAllocation.Key,
                    false, // Désactiver DTLS
                    true   // Activer WebSockets (WSS)
                );

                // Configurer Unity Transport
                UnityTransport transport = FindObjectOfType<UnityTransport>();
                transport.SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }
}