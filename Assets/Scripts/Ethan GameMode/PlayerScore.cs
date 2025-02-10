using Unity.Netcode;
using Unity.Collections;

[System.Serializable]
public struct PlayerScore : INetworkSerializable
{
    public ulong PlayerId;
    public FixedString64Bytes PlayerName;
    public int Score;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Score);
    }
}
