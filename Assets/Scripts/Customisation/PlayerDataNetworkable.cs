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
        public List<int> scoreTable = new();
        public int nbrVictory;
        public int nbrDefeat;
        public List<string> skins = new();
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

            int scoreTableCount = scoreTable.Count;
            serializer.SerializeValue(ref scoreTableCount);
            if (serializer.IsReader)
            {
                scoreTable = new List<int>(scoreTableCount);
            }
            for (int i = 0; i < scoreTableCount; i++)
            {
                int score = scoreTable.Count > i ? scoreTable[i] : 0;
                serializer.SerializeValue(ref score);
                if (serializer.IsReader)
                {
                    scoreTable.Add(score);
                }
            }

            int skinsCount = skins.Count;
            serializer.SerializeValue(ref skinsCount);
            if (serializer.IsReader)
            {
                skins = new List<string>(skinsCount);
            }
            for (int i = 0; i < skinsCount; i++)
            {
                string skin = skins.Count > i ? skins[i] : string.Empty;
                serializer.SerializeValue(ref skin);
                if (serializer.IsReader)
                {
                    skins.Add(skin);
                }
            }
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