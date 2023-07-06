using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        [SerializeField] private bool showPaths = true;
        [SerializeField] private float gizmoRadius = 0.25f;
        [SerializeField] private float offsetScaler = 1f;
        private List<List<GridElement>> _savedPaths = new(); 


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

            SetupGeneration();
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

            Stopwatch stopwatch = new Stopwatch();
            
            stopwatch.Start();
            CreateGround();
            stopwatch.Stop();
            
            Debug.Log($"Creating Ground: {stopwatch.Elapsed}");
            
            stopwatch.Reset();
            stopwatch.Start();
            MakeTunnels(1);
            stopwatch.Stop();
            
            Debug.Log($"Making Tunnels: {stopwatch.Elapsed}");

            
            stopwatch.Reset();
            stopwatch.Start();
            CreateWall();
            stopwatch.Stop();
            
            Debug.Log($"Creating Wall: {stopwatch.Elapsed}");

            
            stopwatch.Reset();
            stopwatch.Start();
            MakeBanAreaOfSurface();
            stopwatch.Stop();
            
            Debug.Log($"Creating Ban Area: {stopwatch.Elapsed}");

            stopwatch.Reset();
            stopwatch.Start();
            for (var height = 0; height < mapSettings.MaxHeight; height++)
            for (var i = 0; i < mapSettings.IterationAmount; i++)
                    SpawnObstacles(height);
            stopwatch.Stop();

            Debug.Log($"Spawning obstacles: {stopwatch.Elapsed}");
        }

        private void CreateGround()
        {
            var groundNoise = mapSettings.GroundPlacementNoise.Generate(X, Z);

            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var grid = _grids[x, 0, z];
                grid.MakeGridCanBeFilledGround();
                SpawnObject(grid, mapSettings.Ground, 0);
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
                    SpawnObject(grid, mapSettings.Ground, 0);
                }
            }
        }

        private void MakeTunnels(int layer)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Reset();
            stopwatch.Start();
            var nodes = CreateTunnelNodes(layer);
            stopwatch.Stop();
            
            Debug.Log($"Creating Tunnel Nodes: {stopwatch.Elapsed}");
            
            stopwatch.Reset();
            stopwatch.Start();
            var pathFinder = CreateTunnelNodeGraph(nodes, layer);
            stopwatch.Stop();
            
            Debug.Log($"Creating Tunnel Node Graph: {stopwatch.Elapsed}");

            stopwatch.Reset();
            stopwatch.Start();
            var paths = FindAllEdgeToEdgePaths(nodes, pathFinder);
            stopwatch.Stop();
            
            Debug.Log($"Finding Edges: {stopwatch.Elapsed}");

            
            if(paths.Count == 0) return;

            
            stopwatch.Reset();
            stopwatch.Start();
            var pathsAsGrids = paths.ConvertAll(input => input.ConvertAll(node => node.GridElement));
            var lenghtFiltered = FilterByHeight(pathsAsGrids);
            var fixedPaths = lenghtFiltered.ConvertAll(FixThePath);
            var heightFiltered = FilterPathsByLenght(fixedPaths);
            var ordered = heightFiltered.OrderByDescending(FindHeightAverageOfPath).ToList();
            stopwatch.Stop();
            
            Debug.Log($"Filtering and converting paths: {stopwatch.Elapsed}");

            
            _savedPaths.Clear();

          
            foreach (var path in ordered)
            {
                var distanceValid = true;
                 foreach (var savedPath in _savedPaths)
                {
                    foreach (var pathGrid in path)
                    {
                        foreach (var savedPathGrid in savedPath)
                        {
                            var distance = (savedPathGrid.Position - pathGrid.Position).sqrMagnitude;
                            if (distance < mapSettings.BetweenTunnelMinSpace)
                            {
                                distanceValid = false;
                            }
                        }
                    }
                }

                 if (distanceValid)
                 {
                     CreateTunnel(path);
                 }
            }
        }

        private void CreateTunnel(List<GridElement> path)
        {
            var start = path.First();
            var end = path.Last();
            var direction = end.Position - start.Position;
            var direction2D = new Vector2Int(direction.x, direction.z);
            var rotationDegree = Mathf.Atan2(direction2D.y, direction2D.x) / Mathf.PI * 180;
            rotationDegree *= -1;
            
            foreach (var pathPoint in path)
            {
                foreach (var tunnelBrushDestroyPoint in mapSettings.TunnelBrush.DestroyPoints)
                {
                    var pos = pathPoint.Position + RotateObstacleVector(rotationDegree, tunnelBrushDestroyPoint);
                    
                    if(IsPosOutsideOfGrids(pos)) continue;
                    
                    var grid = _grids[pos.x, pos.y, pos.z];
                    if(grid.PlacedItem is not TunnelBrush)
                        DestroyGridTunnelVersion(grid);
                    
                }
                
                if (IsPlacableSuitable(pathPoint, mapSettings.TunnelBrush, rotationDegree))
                {
                    SpawnObject(pathPoint, mapSettings.TunnelBrush, rotationDegree);
                }
                

            }
            

            
            _savedPaths.Add(path);
        }

        private List<Node> CreateTunnelNodes(int layer)
        {
            var nodes = new List<Node>();

            for (var x = 1; x < X - 1; x++)
            for (var z = 1; z < Z - 1; z++)
            {
                var grid = _grids[x, layer, z];

                if (grid.GridState == GridState.Filled && IsEdgeGroundYDimensionCheck(grid))
                {
                    nodes.Add(new Node(nodes.Count, NodeState.EdgeGround, grid));
                    continue;
                }

                if (grid.GridState == GridState.Filled)
                {
                    nodes.Add(new Node(nodes.Count, NodeState.Ground, grid));
                    continue;
                }
            }

            return nodes;
        }

        private PathFinder CreateTunnelNodeGraph(List<Node> nodes, int layer)
        {
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

            return pathFinder;
        }

        private List<List<Node>> FindAllEdgeToEdgePaths(List<Node> nodes, PathFinder pathFinder)
        {
            var edgeNodes = nodes.Where(node => node.NodeState == NodeState.EdgeGround).ToList();
            var paths = new List<List<Node>>();
            for (var i = 0; i < edgeNodes.Count - 1; i++)
            {
                var edgeNodeA = edgeNodes[i];
                
                for (var a = i + 1; a < edgeNodes.Count; a++)
                {
                    var edgeNodeB = edgeNodes[a];
                    if ((edgeNodeA.GridElement.Position - edgeNodeB.GridElement.Position).sqrMagnitude <
                        mapSettings.TunnelMinLength) continue;

                    var path = pathFinder.FindShortestPath(edgeNodeA.ID, edgeNodeB.ID);
                    
                    if (path == null) continue;
                    
                    var pathAsNodes = path.ConvertAll(input => nodes.First(node => node.ID == input));
                    paths.Add(pathAsNodes);
                }
            }

            return paths;
        }

        private List<GridElement> FixThePath(List<GridElement> path)
        {
            var start = path.First().Position;
            var end = path.Last().Position;
            var yLayer = start.y;
            
            var newPath = BresenhamLineAlgorithm.DrawLine(new Vector2Int(start.x, start.z), new Vector2Int(end.x, end.z));
            var result = newPath.ConvertAll(input => _grids[input.x, yLayer, input.y]);
            return result;
        }

        private List<List<GridElement>> FilterPathsByLenght(List<List<GridElement>> paths)
        {
            return paths.Where(path =>
                FindLengthOfPath(path) > mapSettings.TunnelMinLength).ToList();
        }
        
        private List<List<GridElement>> FilterByHeight(List<List<GridElement>> paths)
        {
            var result = new List<List<GridElement>>();
            
            foreach (var path in paths)
            {
                var average = FindHeightAverageOfPath(path);
                
                if(average > mapSettings.TunnelAverageMinHeight)
                    result.Add(path);
            }

            return result;
        }

        private float FindHeightAverageOfPath(List<GridElement> path)
        {
            var sum = 0;
            foreach (var gridElement in path)
            {
                var grid = gridElement;
                while (grid.GridState == GridState.Filled)
                {
                    sum++;
                    
                    var pos = grid.Position;
                    pos += Vector3Int.up;
                        
                    if(IsPosOutsideOfGrids(pos))
                        break;
                        
                    grid = _grids[pos.x, pos.y, pos.z];
                }
            }

            var average = (float) sum / path.Count;
            return average;
        }

        private int FindLengthOfPath(List<GridElement> path)
        {
            return (path.First().Position - path.Last().Position).sqrMagnitude;
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
                SpawnObject(grid, mapSettings.Wall, 0);
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
                for (var i = 0; i < 360; i += placable.RotationDegreeStep)
                { 
                    rotatedPlacables.Add(new PlacableData(placable, i));    
                }

            var noise = mapSettings.ObjectPlacementNoise.Generate(X, Z);
            
            foreach (var x in Enumerable.Range(0, X).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(0, Z).OrderBy(_ => UnityEngine.Random.value))
            {
                if(noise[x,z] * 100 < mapSettings.ObjectPlacementThreshold) continue;

                var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();
                
                var grid = _grids[x, layer, z];
                
                foreach (var data in shuffledPlacables)
                {
                    if (!IsPlacableSuitable(grid, data.Placable, data.Rotation)) continue;

                    SpawnObject(grid, data.Placable, data.Rotation);
                    break;
                }
            }
            
        }

        private void SpawnObject(GridElement pos, Placable placable, float rotation)
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
            
            var placableObj = Instantiate(placable, gridParent);
            placableObj.transform.position = pos.GetWorldPosition();

            placableObj.VisualsParent.eulerAngles = Vector3.up * rotation;
            
            /*
            foreach (Transform visual in placableObj.VisualsParent)
            {
                var quaternion = Quaternion.AngleAxis(rotation, Vector3.up);
                visual.localEulerAngles = Vector3.up * rotation;
                visual.localPosition = quaternion * visual.localPosition;
            }
            */
            
            _placedItems.Add(placableObj);
            
            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + RotateObstacleVector(rotation, placableRequiredGrid);
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                grid.FillGrid(placableObj);
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
        }

        private void DestroyGrid(GridElement grid)
        {
            var placable = grid.PlacedItem;
            
            if (grid.PlacedItem == null)
            {
                grid.MakeGridCanBeFilled();
                return;
            }
            
            foreach (var gridElement in _grids)
            {
                if (gridElement.PlacedItem == placable)
                {
                    grid.MakeGridCanBeFilled();
                }
            }

            _placedItems.Remove(placable);
            Destroy(placable.gameObject);
        }
        
        private void DestroyGridTunnelVersion(GridElement grid)
        {
            var placable = grid.PlacedItem;
            
            if (grid.PlacedItem == null)
            {
                grid.MakeGridCanBeFilled();
                return;
            }
            
            grid.MakeGridCanBeFilled();
            _placedItems.Remove(placable);
            Destroy(placable.gameObject);
        }

        private bool IsPlacableSuitable(GridElement pos, Placable placable, float rotation)
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

        private Vector3Int RotateObstacleVector(float angle, Vector3Int vector3Int)
        {
            var rotation = Quaternion.AngleAxis(angle, Vector3.up);
            var result = rotation * vector3Int;
            var resultAsVector3Int = new Vector3Int(Mathf.RoundToInt(result.x), Mathf.RoundToInt(result.y),
                Mathf.RoundToInt(result.z));

            return resultAsVector3Int;
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
            
            if(showPaths)
                foreach (var gridElements in _savedPaths)
                {
                    for (var i = 0; i < gridElements.Count; i++)
                    {
                        var gridElement = gridElements[i];
                        if (i == 0)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere((Vector3) gridElement.Position * offsetScaler, gizmoRadius);
                            continue;
                        }
                    
                        if (i == gridElements.Count - 1)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere((Vector3) gridElement.Position * offsetScaler, gizmoRadius);
                            continue;
                        }
                    
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere((Vector3) gridElement.Position * offsetScaler, gizmoRadius);
                        continue;
                    }
                }

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