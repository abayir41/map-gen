using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public static Vector3Int ToVector3Int(this Vector3 target)
        {
            return new Vector3Int(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
        }
        
        public static Vector3Int RotateVector(this Vector3Int vector3Int, int angle, Vector3 origin)
        {
            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var result = rotation * (vector3Int - origin);
            var resultAsVector3Int = new Vector3Int(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y),
                Mathf.RoundToInt(result.z));

            return resultAsVector3Int;
        }
    }
}