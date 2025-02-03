using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Qos;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network
{
    public class RelayManager : MonoBehaviour
    {
        private async void Start()
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += OnPlayerSignedIn;
            AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        public async Task<string> CreateRelay(int maxPlayers)
        {
            
            try
            {
                (RelayServerData relayServerData, Allocation allocation) = await CreateHostAllocation(maxPlayers);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartServer();
                
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log("server started with join code: " + joinCode);
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return null;
            }
        }
        
        private async Task<(RelayServerData, Allocation)> CreateHostAllocation(int maxPlayer = 4)
        {
            var connectionType = "wss";
            
            Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
            RelayServerData newRelayData = hostAllocation.ToRelayServerData(connectionType);

            return (newRelayData, hostAllocation);
        }
        
        private async Task<RelayServerData> CreateJoinAllocation(string joinCode) 
        {
            string connectionType = "wss";
            
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData newRelayData = joinAllocation.ToRelayServerData(connectionType);

            return newRelayData;
        }

        private void OnPlayerSignedIn()
        {
            Debug.Log("signed in " + AuthenticationService.Instance.PlayerId);
        }

        public async Task JoinRelay(string joinCode)
        {
            await UnityServices.InitializeAsync();

            try
            {
                var relayServerData = await CreateJoinAllocation(joinCode);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
                Debug.Log("client " + AuthenticationService.Instance.PlayerId + " joined relay");
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }
}