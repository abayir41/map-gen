using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGen.Placables
{
    public class Placable : MonoBehaviour
    {
        [Header("Placable Properties")] 
        [SerializeField] private bool _rotatable;
        [Range(1,359)] [SerializeField] private int _rotationDegreeStep = 15;
        public int RotationDegreeStep => Mathf.Clamp(_rotationDegreeStep,1,360);

        [Header("Grid Properties")] 
        [SerializeField] protected List<PlacableGrid> _grids;
        [SerializeField] protected Transform _visualsParent;

        public Transform VisualsParent => _visualsParent;
        public List<PlacableGrid> Grids => _grids;
        public bool Rotatable => _rotatable;

        private void OnValidate()
        {
            _grids = GetComponentsInChildren<PlacableGrid>().ToList();
            _visualsParent = transform.Find("Visuals");
        }
    }
}