using Unity.Netcode;
using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtension
    {
        public static ulong GetNetworkObjectId(this GameObject gameObject)
        {
            var networkObject = gameObject.GetComponent<NetworkObject>();
            return networkObject ? networkObject.NetworkObjectId : ulong.MaxValue;
        }
    }
}