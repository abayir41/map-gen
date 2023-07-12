using System.Collections.Generic;
using MapGen.Placables.GridCreators;
using UnityEngine;

namespace MapGen.Placables
{
    public class HeightBumperGridCreator : GridCreatorMono
    {
        [SerializeField] private GridCreatorMono _referenceCreator;
        [SerializeField] private int _height;
        
        public override List<Vector3Int> GetGrid()
        {
            var grid = _referenceCreator.GetGrid();

            var result = new List<Vector3Int>(grid);
            for (int i = 0; i < _height; i++)
            {
                foreach (var pos in grid)
                {
                    var newPos = pos + Vector3Int.up * i;
                    result.Add(newPos);
                }
            }

            return result;
        }
    }
}