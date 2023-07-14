using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Placables.GridCreators
{
    public class VisualUserGridCreator : GridCreatorMono
    {
        [SerializeField] protected Transform _visualsParent;

        private void OnValidate()
        {
            var possibleVisualParent = transform.root.Find("Visuals");
            if (_visualsParent != possibleVisualParent)
            {
                _visualsParent = possibleVisualParent;
                Debug.Log("Automatically visual parent added");
            }
        }

        public override List<Vector3Int> GetGrid()
        {
            var grid = new List<Vector3Int>();
            foreach (Transform child in _visualsParent)
            {
                grid.Add(Vector3Int.FloorToInt(child.localPosition));
            }

            return grid;
        }
    }
}