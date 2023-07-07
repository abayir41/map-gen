using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGen.Utilities
{
    public static class Extensions
    {
        public static List<T> GetRandomAmountAndShuffled<T>(this List<T> list)
        {
            var shuffled = list.OrderBy(_ => UnityEngine.Random.value).ToList();
            var amount = UnityEngine.Random.Range(0, shuffled.Count);
            var result = shuffled.GetRange(0, amount);
            return result;
        }
        
        public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum
        {
            foreach (T value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}