using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Map;
using MapGen.Placables;
using MapGen.Random;
using MapGen.Utilities;
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
        [SerializeField] private Placable wall;
        [SerializeField] private List<Placable> placables;

        [Header("Gizmo Settings")]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private bool showFilled = true;
        [SerializeField] private bool showLocked = true;
        [SerializeField] private bool showCanBeFilled = true;
        [SerializeField] private bool showCanBeFilledGround = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;
        
        
        
        private int X => mapSettings.X;
        private int Y => mapSettings.Y;
        private int Z => mapSettings.Z;
        
        
        private GridElement[,,] _grids;
        private CustomRandomSettings _cachedRandomSettings;
        private readonly List<Placable> _placedItems = new();


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
            CreateWall();
            MakeBanAreaOfSurface();
            
            for (var i = 0; i < mapSettings.IterationAmount; i++)
                SpawnObstacles(1);
            
            for (var i = 0; i < mapSettings.IterationAmount; i++)
                SpawnObstacles(2);
        }

        private void CreateGround()
        {
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var grid = _grids[x, 0, z];
                grid.MakeGridCanBeFilledGround();
                SpawnObject(grid, ground);
            }
        }

        private void CreateWall()
        {
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var y = 1; y < _grids.GetLength(1); y++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                if (x != 0 && x != _grids.GetLength(0) - 1 && z != 0 && z != _grids.GetLength(2) - 1) continue;
                
                var grid = _grids[x, y, z];
                grid.MakeGridCanBeFilledGround();
                SpawnObject(grid, wall);
            }
        }

        private void MakeBanAreaOfSurface()
        {
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var gridTopTop = _grids[x, _grids.GetLength(1) - 1, z];
                var gridTop = _grids[x, _grids.GetLength(1) - 2, z];
                
                if(gridTopTop.GridState != GridState.Filled)
                    gridTopTop.LockGrid();
                
                if(gridTop.GridState != GridState.Filled)
                    gridTop.LockGrid();
            }
        }

        private void SpawnObstacles(int layer)
        {
            List<PlacableData> placableDataList = new();

            for (int i = 0; i < 4; i++)
            {
                foreach (var placable in placables)
                {
                    var noise = NoiseGenerator.Generate(X, Z, mapSettings.PerlinScale,
                        Vector2.one * mapSettings.PerlinOffsetScale * UnityEngine.Random.value);
                
                    placableDataList.Add(new PlacableData(placable, i * 90, noise));
                }
            }
            

            for (var x = 0; x < X; x++)
            for (var z = 0; z < Z; z++)
            {
                var rankedPlacable = placableDataList
                    .Where(pair => pair.Noise[x, z] * 100 > mapSettings.Threshold)
                    .OrderByDescending(pair => pair.Noise[x, z]).ToList();


                var grid = _grids[x, layer, z];
                
                foreach (var data in rankedPlacable)
                {
                    if (!IsPlacableSuitable(grid, data.Placable, data.Rotation)) continue;

                    SpawnObject(grid, data.Placable, data.Rotation);
                    break;
                }
            }
            
        }

        private void SpawnObject(GridElement pos, Placable placable, int rotation = 0)
        {
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableRequiredGrid);
                if (IsPosOutsideOfGrids(checkedGridPos))
                    throw new Exception("Placing outside of Grid");

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                if (grid.GridState is not (GridState.CanBeFilled or GridState.CanBeFilledGround))
                    throw new Exception("Placing not suitable grid");
            }
            
            foreach (var placableShouldPlacedOnGroundGrid in placable.ShouldPlacedOnGroundGrids)
            {
                var checkedGridPos = pos.Position +  RotateObstacleVector(rotation, placableShouldPlacedOnGroundGrid);

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                if (grid.GridState != GridState.CanBeFilledGround)
                    throw new Exception("Placing not suitable grid");
            }
            
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableRequiredGrid);
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                grid.FillGrid();
            }

            foreach (var placableLockGrid in placable.LockGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableLockGrid);
                if (IsPosOutsideOfGrids(checkedGridPos))
                    continue;
                
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                
                if(grid.GridState != GridState.Filled)
                    grid.LockGrid();
            }
            
            foreach (var placableNewGroundGrid in placable.NewGroundGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableNewGroundGrid);
                if (IsPosOutsideOfGrids(checkedGridPos))
                    continue;
                
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                
                if(grid.GridState is not (GridState.Locked or GridState.Filled))
                    grid.MakeGridCanBeFilledGround();
            }
            
            var placableObj = Instantiate(placable, gridParent);
            placableObj.transform.position = pos.GetWorldPosition();
            placableObj.transform.Rotate(Vector3.up, rotation);
            _placedItems.Add(placableObj);
        }

        private bool IsPlacableSuitable(GridElement pos, Placable placable, int rotation = 0)
        {
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableRequiredGrid);
                if (IsPosOutsideOfGrids(checkedGridPos))
                    return false;

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                if (grid.GridState is not (GridState.CanBeFilled or GridState.CanBeFilledGround))
                    return false;
            }
            
            foreach (var placableShouldPlacedOnGroundGrid in placable.ShouldPlacedOnGroundGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableShouldPlacedOnGroundGrid);

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                if (grid.GridState != GridState.CanBeFilledGround)
                    return false;
            }

            return true;
        }

        private Vector3Int RotateObstacleVector(int rotation, Vector3Int vector3Int)
        {
            var rotated = Quaternion.AngleAxis(rotation, Vector3.up) * vector3Int;
            return Vector3Int.FloorToInt(rotated);
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
            
            UnityEngine.Random.InitState(mapSettings.RandomSettings.GetSeed());
            
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
                    case GridState.CanBeFilledGround when !showCanBeFilledGround:
                        continue;
                }

                var color = grid.GridState switch
                {
                    GridState.Filled => Color.blue,
                    GridState.CanBeFilled => Color.green,
                    GridState.Locked => Color.red,
                    GridState.CanBeFilledGround => Color.yellow,
                    _ => throw new ArgumentOutOfRangeException()
                }; 
                
                Gizmos.color = color;
                Gizmos.DrawSphere((Vector3) grid.Position * offsetScaler, gizmoRadius);
            }
        }
    }
}