using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
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