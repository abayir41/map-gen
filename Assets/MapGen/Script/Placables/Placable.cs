using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        public Vector3Int GridPos { get; private set; }
        
        [Header("Placable Properties")] 
        [SerializeField] private bool _rotatable;
        [Range(1,359)] [SerializeField] private int _rotationDegreeStep = 15;

        public int RotationDegreeStep => Mathf.Clamp(_rotationDegreeStep,1,360);

        [Header("Grid Properties")] 
        [SerializeField] protected List<PlacableGrid> _grids;
        [SerializeField] protected Transform _visualsParent;
        
        public List<PlacableGrid> Grids => _grids;
        public bool Rotatable => _rotatable;

        public void InitializePlacable(Vector3Int gridPos)
        {
            GridPos = gridPos;
        }

        private void OnValidate()
        {
            _grids = GetComponentsInChildren<PlacableGrid>().ToList();
            
            var possibleVisualParent = transform.root.Find("Visuals");
            if (_visualsParent != possibleVisualParent)
            {
                _visualsParent = possibleVisualParent;
                Debug.Log("Automatically visual parent added");
            }
        }

        public void Rotate(float degree)
        {
            _visualsParent.eulerAngles = Vector3.up * degree;
        }
    }
}