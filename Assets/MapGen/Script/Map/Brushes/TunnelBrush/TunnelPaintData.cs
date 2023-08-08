using System.Collections.Generic;
using MapGen.Placables;

namespace MapGen.Map.Brushes.TunnelBrush
{
    public struct TunnelPaintData
    {
        public List<SpawnData> NewSpawnedObjects { get; }
        public List<SpawnData> DestroyedObjects { get; }
        
        public TunnelPaintData(List<SpawnData> destroyedObjects, List<SpawnData> newSpawnedObjects)
        {
            DestroyedObjects = destroyedObjects;
            NewSpawnedObjects = newSpawnedObjects;
        }
    }
}