using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Extensions
{
    public static class ListExtensions
    {
        public static T PickRandom<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new InvalidOperationException("Cannot pick a random element from an empty or null list.");
            }

            int index = Random.Range(0, list.Count);
            return list[index];
        }
    }
}