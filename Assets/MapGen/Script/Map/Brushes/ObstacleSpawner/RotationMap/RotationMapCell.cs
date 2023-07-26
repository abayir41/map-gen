using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Map
{
    public struct RotationMapCell
    {
        public List<int> Rotations { get; }

        public RotationMapCell(params int[] rotations)
        {
            Rotations = new List<int>();
            foreach (var rotation in rotations)
            {
                Rotations.Add(rotation);
            }
        }
    }
}