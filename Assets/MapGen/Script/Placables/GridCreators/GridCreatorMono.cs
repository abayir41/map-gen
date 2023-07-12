using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
{
    public abstract class GridCreatorMono : MonoBehaviour
    {
        public abstract List<Vector3Int> GetGrid();
    }
}