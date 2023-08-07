using System.Linq;
using MapGen.Map;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.TunnelSystem
{
    public class TunnelPlacable : Placable
    {
        [SerializeField] private GameObject _tunnelPrefab;
        [SerializeField] private Transform _tunnelPrefabParent;
        public override void InitializePlacable(SpawnData spawnData)
        {
            base.InitializePlacable(spawnData);

            var physicalsGrids = _grids.Where(grid => grid.PlacableCellType == PlacableCellType.PhysicalVolume);
            foreach (var placableGrid in physicalsGrids)
            {
                foreach (var cellPos in placableGrid.CellPositions)
                {
                    var rotatedVector = cellPos.RotateVector(spawnData.Rotation, Origin);
                    var instantiated = Instantiate(_tunnelPrefab, _tunnelPrefabParent);
                    instantiated.transform.localPosition = rotatedVector;
                }
            }
        }
    }
}