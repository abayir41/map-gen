using System.Collections.Generic;
using MapGen.Placables.GridCreators;
using UnityEngine;

namespace MapGen.Placables
{
    public class NormalGridCreator : GridCreatorMono 
    {
        [SerializeField] private List<Vector3Int> positions;
        public override List<Vector3Int> GetGrid()
        {
            return positions;
        }
    }
}