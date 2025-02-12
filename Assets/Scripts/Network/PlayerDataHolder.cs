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
        }

        [Rpc(SendTo.Server)]
        public void SetColorServerRpc(string newColor)
        {
            Debug.Log("received color: " + newColor);
            Color.Value = newColor;
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
    }
}