using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;

namespace Customisation
{
    [Serializable]
    public class PlayerDataNetworkable : INetworkSerializable, IEquatable<PlayerDataNetworkable>
    {
        public ulong ClientId;
        
        public int id;
        public FixedString32Bytes pseudo;
        public int highScore;
        public int nbrVictory;
        public int nbrDefeat;
        public FixedString32Bytes color;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref pseudo);
            serializer.SerializeValue(ref highScore);
            serializer.SerializeValue(ref nbrVictory);
            serializer.SerializeValue(ref nbrDefeat);
            serializer.SerializeValue(ref color);
            serializer.SerializeValue(ref ClientId);
        }

        public bool Equals(PlayerDataNetworkable other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id && pseudo == other.pseudo && highScore == other.highScore && nbrVictory == other.nbrVictory && nbrDefeat == other.nbrDefeat && color == other.color;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PlayerDataNetworkable)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, pseudo, highScore, nbrVictory, nbrDefeat, color);
        }
    }
}