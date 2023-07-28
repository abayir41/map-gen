using MapGen.Placables;
using UnityEngine;

namespace MapGen.GridSystem
{
    public class SelectableGridCell : MonoBehaviour
    {
        public GridCell BoundedCell { get; set; }
        public Placable BoundedPlacable { get; set; }
    }
}