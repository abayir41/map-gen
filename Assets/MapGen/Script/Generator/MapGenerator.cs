using System;
using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map;
using MapGen.Placables;
using MapGen.Random;
using Unity.VisualScripting;
using UnityEngine;

namespace MapGen.Generator
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private MapSettings mapSettings;

        [SerializeField] private Transform gridParent;
        
        [Header("Prefabs")]
        [SerializeField] private Placable ground;
        
        
        [Header("Gizmo Settings")]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private bool showNeutral = false;
        [SerializeField] private bool showFilled = true;
        [SerializeField] private bool showCanBeFilled = true;
        [SerializeField] private bool showLocked = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;
        
        private int X => mapSettings.X;
        private int Y => mapSettings.Y;
        private int Z => mapSettings.Z;
        
        
        private GridElement[,,] _grids;
        private CustomRandomSettings _cachedRandomSettings;
        private List<Placable> _placedItems = new();


        private void Awake()
        {
            mapSettings.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged()
        {
            SetupGeneration();
            Generate();
        }

        private void Start()
        {
            SetupGeneration();
            Generate();
        }

        private void Generate()
        {
            Debug.Log("Generation Started");
            CreateGround();
        }

        private void CreateGround()
        {
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var grid = _grids[x, 0, z];
                grid.MakeGridCanBeFilled();
                SpawnObject(grid, ground);
            }
        }

        private void SpawnObject(GridElement pos, Placable placable)
        {
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + placableRequiredGrid;
                if (IsPosOutsideOfGrids(checkedGridPos))
                    throw new Exception("Placing outside of Grid");

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                if (grid.GridState != GridState.CanBeFilled)
                    throw new Exception("Placing not suitable grid");
            }
            
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + placableRequiredGrid;
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                grid.FillGrid();
            }

            var placableObj = Instantiate(placable, gridParent);
            placableObj.transform.position = pos.GetWorldPosition();
            
            foreach (var placableLockGrid in placable.LockGrids)
            {
                var checkedGridPos = pos.Position + placableLockGrid;
                if (IsPosOutsideOfGrids(checkedGridPos))
                    continue;
                
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                grid.LockGrid();
            }
            
            foreach (var placableUnlockGrid in placable.UnlockGrids)
            {
                var checkedGridPos = pos.Position + placableUnlockGrid;
                if (IsPosOutsideOfGrids(checkedGridPos))
                    continue;
                
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                
                if(grid.GridState == GridState.Neutral)
                    grid.MakeGridCanBeFilled();
            }
            
            _placedItems.Add(placableObj);
            
        }

        private bool IsPosOutsideOfGrids(Vector3Int pos)
        {
            return pos.x >= _grids.GetLength(0) || pos.x < 0 || 
                   pos.y >= _grids.GetLength(1) || pos.y < 0 ||
                   pos.z >= _grids.GetLength(2) || pos.z < 0;
        }

        private void SetupGeneration()
        {
            foreach (var placedItem in _placedItems)
            {
                Destroy(placedItem.gameObject);
            }
            
            _placedItems.Clear();

            _grids = new GridElement[X,Y,Z];
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var y = 0; y < _grids.GetLength(1); y++)
            for (var z = 0; z < _grids.GetLength(2); z++)
                _grids[x, y, z] = new GridElement(x, y, z);
            
            
            if (mapSettings.RandomSettings is CustomRandomSettings customRandomSettings && customRandomSettings != _cachedRandomSettings)
            {
                _cachedRandomSettings = customRandomSettings;
                _cachedRandomSettings.PropertyChanged += PropertyChanged;
            }
        }
        
        void OnDrawGizmos()
        {
            if(_grids == null) return;
            if(!drawGizmo) return;
            
            
            foreach (var grid in _grids)
            {
                switch (grid.GridState)
                {
                    case GridState.Filled when !showFilled:
                    case GridState.Locked when !showLocked:
                    case GridState.CanBeFilled when !showCanBeFilled:
                    case GridState.Neutral when !showNeutral:
                        continue;
                }

                var color = grid.GridState switch
                {
                    GridState.Filled => Color.blue,
                    GridState.Neutral => Color.gray,
                    GridState.CanBeFilled => Color.green,
                    GridState.Locked => Color.red,
                    _ => throw new ArgumentOutOfRangeException()
                }; 
                
                Gizmos.color = color;
                Gizmos.DrawSphere((Vector3) grid.Position * offsetScaler, gizmoRadius);
            }
        }
    }
}