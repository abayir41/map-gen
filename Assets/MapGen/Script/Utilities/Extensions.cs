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
    }
}