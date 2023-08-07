using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.Command;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush", menuName = "MapGen/Brushes/Obstacle/Brush", order = 0)]
    public class PlacableSpawner : SingleCellEditableBrush
    {
        [SerializeField] private EndlessList<Placable> _placables;
        [SerializeField] private PlacableCellType _visualShownCellType;
        [SerializeField] private int _rotationStep = 15;
        
        public override string BrushName => "Placable";
        protected override int HitBrushHeight => 1;
        public int Rotation => _rotation;
        public string CurrentPlacableName => _placables.CurrentItem.Name;

        private List<Vector3Int> _placableVisuals = new();
        private Color _placableVisualColor = Color.green;
        private Color _placableEdgeColor = Color.blue;
        private Color _cursorColor = Color.yellow;
        private int _rotation;
        
        public override void Update()
        {
            base.Update();
            
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    NextPlacable();
                }
                else
                {
                    NextRotatePlacable();
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PreviousPlacable();
                }
                else
                {
                    PreviousRotatePlacable();
                }
            }

            var grid = _placables.CurrentItem.Grids.FirstOrDefault(grid => grid.PlacableCellType == _visualShownCellType);
            if (grid == null) return;

            _placableVisuals = grid.TransformAccordingToSpawn(_placables.CurrentItem, HitPosOffsetted, _rotation);
            
            
            if (WorldCreator.Grid.IsPlacableSuitable(HitPosOffsetted, _placables.CurrentItem, _rotation))
            {
                _placableVisualColor = Color.green;
            }
            else
            {
                _placableVisualColor = Color.red;
            }
            
        }
        
        public void NextRotatePlacable()
        {
            _rotation += _rotationStep;
                
            if (_rotation > 360)
            {
                _rotation = 0;
            }
            else if (_rotation < 0)
            {
                _rotation = 360;
            }
        }

        public void PreviousRotatePlacable()
        {
            _rotation -= _rotationStep;

            if (_rotation > 360)
            {
                _rotation = 0;
            }
            else if (_rotation < 0)
            {
                _rotation = 360;
            }
        }

        public void NextPlacable()
        {
            _placables.NextItem();
        }

        public void PreviousPlacable()
        {
            _placables.PreviousItem();
        }

        public override void OnDrawGizmos()
        { 
            base.OnDrawGizmos();
            
            
            Gizmos.color = _cursorColor;
            var cursor = WorldCreator.Grid.CellPositionToRealWorld(HitPosOffsetted);
            Gizmos.DrawWireCube(cursor, Vector3.one);
            
            
            Gizmos.color = _placableEdgeColor;
            var filters = _placables.CurrentItem.GetComponentsInChildren<MeshFilter>();
            foreach (var filter in filters)
            {
                var mesh = filter.sharedMesh;
                var transform = filter.transform;
                var pos = transform.position;
                var rotatedPos = pos.RotateVector(_rotation, transform.parent.position);
                Gizmos.DrawMesh(mesh, -1, rotatedPos + cursor, Quaternion.AngleAxis(_rotation, Vector3.up));
            }
            
            Gizmos.color = _placableVisualColor;
            foreach (var placableVisual in _placableVisuals)
            {
                var left = placableVisual + Vector3Int.left;
                var right = placableVisual + Vector3Int.right;
                var top = placableVisual + Vector3Int.up;
                var bottom = placableVisual + Vector3Int.down;
                
                
                
                if(_placableVisuals.Contains(left) && _placableVisuals.Contains(right) && _placableVisuals.Contains(bottom) && _placableVisuals.Contains(top)) continue;
                
                var pos = WorldCreator.Grid.CellPositionToRealWorld(placableVisual);
                Gizmos.DrawWireCube(pos, Vector3.one);
            }
            
           
        }

        public override ICommand GetPaintCommand(Vector3Int startPoint, Grid grid)
        {
            return new PlacableSpawnerCommand(this, WorldCreator.Instance, startPoint, grid);
        }

        public SpawnData? Paint(Vector3Int startPoint, Grid grid)
        {
            if (!WorldCreator.Instance.Grid.IsPlacableSuitable(startPoint, _placables.CurrentItem, _rotation)) return null;

            var data = new SpawnData(startPoint, _placables.CurrentItem, _rotation, CellLayer.Obstacle);
            WorldCreator.Instance.SpawnObject(data);

            return data;
        }
    }
}