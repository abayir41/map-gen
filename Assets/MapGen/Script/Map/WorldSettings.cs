﻿using UnityEngine;

namespace MapGen.Map
{
    [CreateAssetMenu(fileName = "World Settings", menuName = "MapGen/World/Settings", order = 0)]
    public class WorldSettings : ScriptableObject
    {
        public static readonly Vector3 PlaneStartHeight = new Vector3(0, 0, 0);
        public static readonly Vector3 PlaneStartNormal = Vector3.up;
        public static Vector3Int GridCellRealWorldSize = Vector3Int.one;
    }
}