using System;
using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using UnityEngine;

namespace MapGen.Map
{
    public class MapGenerator : MonoBehaviour
    {
        ////////////
        /// Editor
        ///////////
        [SerializeField] [HideInInspector] public bool mapSettingsFoldout;

        
        [SerializeField] private MapSettings mapSettings;
        [SerializeField] private Transform gridParent;
        [SerializeField] private bool editorAutoGenMap;
        
        
        [Header("Gizmo Settings")]
        [SerializeField] private bool drawGizmo = true;
        [SerializeField] private bool showFilled = true;
        [SerializeField] private bool showLocked = true;
        [SerializeField] private bool showCanBeFilled = true;
        [SerializeField] private bool showCanBeFilledGround = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;

        [SerializeField] private float asdasd;
        private Vector3Int _startAsd;
        private Vector3Int _endAsd;


        public MapSettings MapSettings => mapSettings;
        private int X => mapSettings.X;
        private int Y => mapSettings.Y;
        private int Z => mapSettings.Z;
        
        
        private GridElement[,,] _grids;
        private readonly List<Placable> _placedItems = new();

        private void Start()
        {
            GenerateMap();
        }
        

        public void GenerateMapAuto()
        {
            if(!Application.isPlaying) return;
            if(!editorAutoGenMap) return;
            
            GenerateMap();
        }

        public void GenerateMap()
        {
            if(!Application.isPlaying) return;

            SetupGeneration();
            Generate();
        }

        private void Generate()
        {
            Debug.Log("Generation Started");
            CreateGround();
            MakeTunnels(2);
            CreateWall();
            MakeBanAreaOfSurface();

            for (var height = 0; height < mapSettings.MaxHeight; height++)
            for (var i = 0; i < mapSettings.IterationAmount; i++)
                    SpawnObstacles(height);
            
        }

        private void CreateGround()
        {
            var groundNoise = mapSettings.GroundPlacementNoise.Generate(X, Z);

            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var grid = _grids[x, 0, z];
                grid.MakeGridCanBeFilledGround();
                SpawnObject(grid, mapSettings.Ground);
            }
            
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var height = groundNoise[x, z] * mapSettings.GroundHeightFactor - mapSettings.GroundMoveDownFactor;
                
                for (var y = 1; y <  height; y++)
                {
                    if(groundNoise[x,z] * 100 < height) break;
                    var grid = _grids[x, y, z];
                    grid.MakeGridCanBeFilledGround();
                    SpawnObject(grid, mapSettings.Ground);
                }
            }
        }

        private void MakeTunnels(int layer)
        {
            var nodes = new List<Node>();

            for (var x = 1; x < X - 1; x++)
            for (var z = 1; z < Z - 1; z++)
            {
                var grid = _grids[x, layer, z];

                if (grid.GridState == GridState.Filled && IsEdgeGroundYDimensionCheck(grid))
                {
                    Debug.Log("asd");
                    nodes.Add(new Node(nodes.Count, NodeState.EdgeGround, grid));
                    continue;
                }

                if (grid.GridState == GridState.Filled)
                {
                    Debug.Log("asdasd");
                    nodes.Add(new Node(nodes.Count, NodeState.Ground, grid));
                    continue;
                }
            }
            
            var pathFinder = new PathFinder(nodes.Count);

            for (var x = 0; x < X - 1; x++)
            for (var z = 0; z < Z - 1; z++)
            {
                var grid = _grids[x, layer, z];
                
                if(nodes.All(node => node.GridElement != grid))
                    continue;
                
                var nodeGrid = nodes.First(node => node.GridElement == grid);
                var right = _grids[x + 1, layer, z];
                var bottom = _grids[x, layer, z + 1];

                if (nodes.Any(node => node.GridElement == right))
                {
                    var rightGrid = nodes.First(node => node.GridElement == right);
                    pathFinder.AddEdge(rightGrid.ID, nodeGrid.ID);
                }
                
                if (nodes.Any(node => node.GridElement == bottom))
                {
                    var bottomGrid = nodes.First(node => node.GridElement == bottom);
                    pathFinder.AddEdge(bottomGrid.ID, nodeGrid.ID);
                }
            }

            var edgeNodes = nodes.Where(node => node.NodeState == NodeState.EdgeGround).ToList();
            var paths = new List<List<int>>();
            for (var i = 0; i < edgeNodes.Count - 1; i++)
            {
                for (var a = 0; a < edgeNodes.Count; a++)
                {
                    var path = pathFinder.PrintShortestDistance(edgeNodes[i].ID, edgeNodes[a].ID);
                    if(path != null)
                        paths.Add(path);
                }
            }

            var ordered = paths.OrderByDescending(ints => ints.Count).ToList();
            foreach (var path in ordered)
            {
                var start = nodes.First(node => node.ID == path[0]);
                var end = nodes.First(node => node.ID == path[^1]);

                _startAsd = start.GridElement.Position;
                _endAsd = end.GridElement.Position;
                
                Debug.Log("Path Lenght: "+ path.Count + " Start Node Pos: " + start.GridElement.Position + " End Node Pos: " + end.GridElement.Position);
                break;
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
                SpawnObject(grid, mapSettings.Wall);
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
            var rotatedPlacables = new List<PlacableData>();

            foreach (var placable in mapSettings.Placables)
                for (var i = 0; i < 4; i++)
                { 
                    rotatedPlacables.Add(new PlacableData(placable, i * 90));    
                }

            var noise = mapSettings.ObjectPlacementNoise.Generate(X, Z);
            
            foreach (var x in Enumerable.Range(0, X).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(0, Z).OrderBy(_ => UnityEngine.Random.value))
            {
                if(noise[x,z] * 100 < mapSettings.ObjectPlacementThreshold) continue;

                var shuffledPlacables = rotatedPlacables.OrderBy(_ => UnityEngine.Random.value);
                
                var grid = _grids[x, layer, z];
                
                foreach (var data in shuffledPlacables)
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
            for (var i = 0; i < rotation / 90; i++)
            {
                vector3Int = new Vector3Int(vector3Int.z, vector3Int.y, -vector3Int.x);
            }

            return vector3Int;
        }

        private bool IsPosOutsideOfGrids(Vector3Int pos)
        {
            return pos.x >= _grids.GetLength(0) || pos.x < 0 || 
                   pos.y >= _grids.GetLength(1) || pos.y < 0 ||
                   pos.z >= _grids.GetLength(2) || pos.z < 0;
        }

        private bool IsEdgeGroundYDimensionCheck(GridElement element)
        {
            var pos = element.Position;
            
            var offset = pos + Vector3Int.right;
            var neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrids(offset) && neighborGrid.GridState is GridState.CanBeFilled or GridState.CanBeFilledGround)
                return true;
            
            offset = pos + Vector3Int.left;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrids(offset) && neighborGrid.GridState is GridState.CanBeFilled or GridState.CanBeFilledGround)
                return true;
            
            offset = pos + Vector3Int.forward;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrids(offset) && neighborGrid.GridState is GridState.CanBeFilled or GridState.CanBeFilledGround)
                return true;
            
            offset = pos + Vector3Int.back;
            neighborGrid = _grids[offset.x, offset.y, offset.z];
            if (!IsPosOutsideOfGrids(offset) && neighborGrid.GridState is GridState.CanBeFilled or GridState.CanBeFilledGround)
                return true;

            return false;
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

            UnityEngine.Random.InitState(mapSettings.RandomSettings.GetSeed());
            
        }
        
        void OnDrawGizmos()
        {
            if(_grids == null) return;
            if(!drawGizmo) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere((Vector3) _startAsd * offsetScaler, asdasd);
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere((Vector3) _endAsd * offsetScaler, asdasd);
            
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