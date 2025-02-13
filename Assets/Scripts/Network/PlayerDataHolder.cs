using Customisation;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class PlayerDataHolder : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> Pseudo = new();
        public NetworkVariable<FixedString32Bytes> Color = new();

        [Rpc(SendTo.Server)]
        public void SetPseudoServerRpc(string newPseudo)
        {
            Debug.Log("received pseudo: " + newPseudo);
            Pseudo.Value = newPseudo;
            RefreshEveryoneClientRpc();
        }

        [Rpc(SendTo.Server)]
        public void SetColorServerRpc(string newColor)
        {
            Debug.Log("received color: " + newColor);
            Color.Value = newColor;
            RefreshEveryoneClientRpc();
        }

        public PlayerDataNetworkable GetPlayerData()
        {
            var playerData = new PlayerDataNetworkable()
            {
                pseudo = Pseudo.Value,
                color = Color.Value
            };
            return playerData;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void RefreshEveryoneServerRpc()
        {
            RefreshEveryoneClientRpc();
        }

        [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
        public void RefreshEveryoneClientRpc()
        {
            PlayerDataManager.Datainstance.Invoke(nameof(PlayerDataManager.Datainstance.RefreshAllPlayers), 2);
        }
    }
}