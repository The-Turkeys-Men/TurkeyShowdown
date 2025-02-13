using System;
using Unity.Netcode;
using Unity.Collections;

[Serializable]
public class PlayerScore : INetworkSerializable, IEquatable<PlayerScore>
{
    public ulong PlayerId;
    public int Score;

    // Serialize the structure for network transmission
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Score);
    }

    // Compare two PlayerScore instances based on PlayerId and Score
    public bool Equals(PlayerScore other) => PlayerId == other.PlayerId && Score == other.Score;
    
    public override bool Equals(object obj) => obj is PlayerScore other && Equals(other);

    // Generate a hash code based on PlayerId and Score
    public override int GetHashCode() => HashCode.Combine(PlayerId, Score);
}
