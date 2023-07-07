using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using Plugins.Utilities;
using UnityEngine;
using Weaver;
using Debug = UnityEngine.Debug;

namespace MapGen.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private MapSettings _mapSettings;
        [SerializeField] private Transform _gridParent;

        [Header("Gizmo Settings")]
        [SerializeField] private MapGizmos _mapGizmos;
        [SerializeField] private float _gizmoRadius = 0.25f;
        [SerializeField] private float _offsetScaler = 1f;
        

        public MapSettings MapSettings => _mapSettings;
        private int X => _mapSettings.MapSize.x;
        private int Y => _mapSettings.MapSize.y;
        private int Z => _mapSettings.MapSize.z;

        private GridHelper _gridHelper;
        private GridElement[,,] _grids;
        private List<Placable> _placedItems = new();
        private List<List<GridElement>> _cachedPaths = new();
        private List<List<Node>> _edgeGroups = new();

        /// <summary>
        /// This function called from Editor, Look at: "MapGeneratorEditor.cs"
        /// </summary>
        [MethodTimer]
        public void GenerateMap()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Method Should be called when game is playing, It can't be called from Editor");
                return;
            }

            SetupGeneration();
            Generate();
        }

        private void SetupGeneration()
        {
            DestroyPlacedPlacables();
            ClearLists();
            CreateGridSystem();
            SetRandomSeed();
        }

        private void DestroyPlacedPlacables()
        {
            foreach (var placedItem in _placedItems) Destroy(placedItem.gameObject);
        }

        private void ClearLists()
        {
            _placedItems.Clear();
            _cachedPaths.Clear();
            _edgeGroups.Clear();
        }

        private void CreateGridSystem()
        {
            _grids = new GridElement[X, Y, Z];
            _gridHelper = new GridHelper(_grids, _mapSettings);
            var xDimension = _grids.GetLength(0);
            var yDimension = _grids.GetLength(1);
            var zDimension = _grids.GetLength(2);

            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    for (var z = 0; z < zDimension; z++)
                    {
                        _grids[x, y, z] = new GridElement(x, y, z);
                    }
                }
            }
        }

        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(_mapSettings.RandomSettings.GetSeed());
        }
        
        private void Generate()
        {
            Debug.Log("Generation Started");
            if (_mapSettings.MapParts.HasFlag(MapParts.Ground))
            {
                CreateGround();
            }

            if (_mapSettings.MapParts.HasFlag(MapParts.Mountains))
            {
                CreateMountains();
            }

            if (_mapSettings.MapParts.HasFlag(MapParts.Tunnels))
            {
                MakeTunnels(_mapSettings.TunnelYLevel);
            }

            if (_mapSettings.MapParts.HasFlag(MapParts.Walls))
            {
                CreateWall();
            }
            
            MakeBanAreaOfSurface();

            if (_mapSettings.MapParts.HasFlag(MapParts.Obstacles))
            {
                for (var height = 0; height < _mapSettings.ObstaclesMaxHeight; height++)
                {
                    for (var i = 0; i < _mapSettings.IterationAmount; i++)
                    {
                        SpawnObstacles(height);
                    }
                }
            }
            
        }

        [MethodTimer]
        private void CreateGround()
        {
            var xDimension = _grids.GetLength(0);
            var zDimension = _grids.GetLength(2);

            for (var x = 0; x < xDimension; x++)
            {
                for (var z = 0; z < zDimension; z++)
                {
                    var grid = _grids[x, 0, z];
                    grid.MakeGridCanBeFilledGround();
                    SpawnObject(grid, _mapSettings.Ground, GridLayer.Ground, 0);
                }
            }
        }

        [MethodTimer]
        private void CreateMountains()
        {
            var mountains = _mapSettings.GroundPlacementNoise.Generate(X, Z);

            var xDimension = _grids.GetLength(0);
            var zDimension = _grids.GetLength(2);
            
            for (var x = 0; x < xDimension; x++)
            {
                for (var z = 0; z < zDimension; z++)
                {
                    var height = mountains[x, z] * _mapSettings.GroundHeightFactor -
                                 _mapSettings.GroundMoveDownFactor;

                    for (var y = 1; y < height; y++)
                    {
                        if (ZeroOneIntervalToPercent(mountains[x, z]) < height) break;
                        var grid = _grids[x, y, z];
                        grid.MakeGridCanBeFilledGround();
                        SpawnObject(grid, _mapSettings.Ground, GridLayer.Ground, 0);
                    }
                }
            }
        }

        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }

        [MethodTimer]
        private void MakeTunnels(int layer)
        {
            var nodes = CreateTunnelNodes(layer);
            var pathFinder = CreateTunnelNodeGraph(nodes, layer);
            var edges = nodes.Where(node => node.NodeState == NodeState.EdgeGround).ToList();
            var edgeGroups = GroupEdgeNodes(edges);
            _edgeGroups = edgeGroups;
            var tunnelPaths = FindAllEdgeToEdgePathsFromGroups(edgeGroups, nodes, pathFinder);

            if (tunnelPaths.Count == 0) return;

            var pathsAsGrids = tunnelPaths.ConvertAll(input => input.ConvertAll(node => node.GridElement));
            var lenghtFilteredTunnels = _gridHelper.FilterByHeight(pathsAsGrids);
            var fixedTunnelPaths = lenghtFilteredTunnels.ConvertAll(FixThePath);
            var heightFilteredTunnels = _gridHelper.FilterPathsByLenght(fixedTunnelPaths);
            var orderedTunnels = heightFilteredTunnels.OrderByDescending(_gridHelper.FindHeightAverageOfPath).ToList();
            
            foreach (var path in orderedTunnels)
            {
                if (_gridHelper.CanTunnelSpawnable(path, _cachedPaths)) CreateTunnel(path);
            }
        }

        [MethodTimer]
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
                foreach (var tunnelBrushDestroyPoint in _mapSettings.TunnelBrush.DestroyPoints)
                {
                    var pos = pathPoint.Position +
                              _gridHelper.RotateObstacleVector(rotationDegree, tunnelBrushDestroyPoint);

                    if (_gridHelper.IsPosOutsideOfGrids(pos)) continue;

                    var grid = _grids[pos.x, pos.y, pos.z];
                    if (grid.PlacedItem is TunnelBrush) continue;
                    
                    var ground = grid.PlacedItem;
                    grid.FreeTheGrid();
                    grid.MakeGridEmpty();
                    
                    if (grid.PlacedItem == null) continue;
                    
                    _placedItems.Remove(ground);
                    Destroy(ground.gameObject);
                }

                if (_gridHelper.IsPlacableSuitable(pathPoint, _mapSettings.TunnelBrush, rotationDegree))
                    SpawnObject(pathPoint, _mapSettings.TunnelBrush, GridLayer.Obstacle, rotationDegree);
            }
            
            _cachedPaths.Add(path);
        }

        [MethodTimer]
        private List<Node> CreateTunnelNodes(int layer)
        {
            var nodes = new List<Node>();

            for (var x = 1; x < X - 1; x++)
            for (var z = 1; z < Z - 1; z++)
            {
                var grid = _grids[x, layer, z];

                if (grid.GridState == GridState.Filled && _gridHelper.IsEdgeGroundYDimensionCheck(grid))
                {
                    nodes.Add(new Node(nodes.Count, NodeState.EdgeGround, grid));
                    continue;
                }

                if (grid.GridState == GridState.Filled) nodes.Add(new Node(nodes.Count, NodeState.Ground, grid));
            }

            return nodes;
        }

        [MethodTimer]
        private PathFinder CreateTunnelNodeGraph(List<Node> nodes, int layer)
        {
            var pathFinder = new PathFinder(nodes.Count);

            for (var x = 0; x < X - 1; x++)
            for (var z = 0; z < Z - 1; z++)
            {
                var grid = _grids[x, layer, z];

                if (nodes.All(node => node.GridElement != grid))
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

        [MethodTimer]
        private List<List<Node>> GroupEdgeNodes(List<Node> edgeNodes)
        {
            var result = new List<List<Node>>();
            var remainingNodes = new List<Node>(edgeNodes);
            
            foreach (var node in edgeNodes)
            {
                var processNode = true;
                foreach (var list in result)
                {
                    if (list.Contains(node))
                    {
                        processNode = false;
                        break;
                    }
                }

                if (!processNode) continue;

                var group = new List<Node> {node};
                FindAllNeighborEdges(node, remainingNodes, group);
                remainingNodes = remainingNodes.Except(group).ToList();
                result.Add(group);
            }

            return result;
        }

        private void FindAllNeighborEdges(Node start, List<Node> nodes, List<Node> result)
        {
            for (var xAxis = -1; xAxis <= 1; xAxis++)
            {
                for (var yAxis = -1; yAxis <= 1; yAxis++)
                {
                    var possibleNeighbor = nodes.FirstOrDefault(node =>
                        node.GridElement.Position == start.GridElement.Position + new Vector3Int(xAxis, 0, yAxis));

                    if (possibleNeighbor == null || result.Contains(possibleNeighbor)) continue;
                        
                    result.Add(possibleNeighbor);
                    FindAllNeighborEdges(possibleNeighbor, nodes, result);
                }
            }
        }

        [MethodTimer]
        private List<List<Node>> FindAllEdgeToEdgePathsFromGroups(List<List<Node>> edgeGroups, List<Node> allNodes, PathFinder pathFinder)
        {
            var paths = new List<List<Node>>();

            Parallel.ForEach(edgeGroups, edgeGroup =>
            {
                Parallel.For(0, edgeGroup.Count - 1, i =>
                {
                    var edgeNodeA = edgeGroup[i];

                    Parallel.For(i + 1, edgeGroup.Count, a =>
                    {
                        var edgeNodeB = edgeGroup[a];
                        if ((edgeNodeA.GridElement.Position - edgeNodeB.GridElement.Position).sqrMagnitude <
                            _mapSettings.TunnelMinLength) return;

                        var path = pathFinder.FindShortestPath(edgeNodeA.ID, edgeNodeB.ID);

                        if (path == null) return;

                        var pathAsNodes = path.ConvertAll(input => allNodes.First(node => node.ID == input));
                        paths.Add(pathAsNodes);
                    });
                });
            });
            return paths;
        }

        private List<GridElement> FixThePath(List<GridElement> path)
        {
            var start = path.First().Position;
            var end = path.Last().Position;
            var yLayer = start.y;

            var newPath =
                BresenhamLineAlgorithm.DrawLine(new Vector2Int(start.x, start.z), new Vector2Int(end.x, end.z));
            var result = newPath.ConvertAll(input => _grids[input.x, yLayer, input.y]);
            return result;
        }

        [MethodTimer]
        private void CreateWall()
        {
            var xDimension = _grids.GetLength(0);
            var yDimension = _grids.GetLength(1);
            var zDimension = _grids.GetLength(2);

            for (var x = 0; x < xDimension; x++)
            {
                for (var y = 0; y < yDimension; y++)
                {
                    for (var z = 0; z < zDimension; z++)
                    {
                        if (x != 0 && x != _grids.GetLength(0) - 1 && z != 0 && z != _grids.GetLength(2) - 1) continue;

                        var grid = _grids[x, y, z];
                        grid.MakeGridCanBeFilledGround();
                        SpawnObject(grid, _mapSettings.Wall, GridLayer.Obstacle, 0);
                    }
                }
            }
        }

        [MethodTimer]
        private void MakeBanAreaOfSurface()
        {
            var xDimension = _grids.GetLength(0);
            var zDimension = _grids.GetLength(2);

            for (var x = 0; x < xDimension; x++)
            {
                for (var z = 0; z < zDimension; z++)
                {
                    var gridTopTop = _grids[x, _grids.GetLength(1) - 1, z];
                    var gridTop = _grids[x, _grids.GetLength(1) - 2, z];

                    if (gridTopTop.GridState != GridState.Filled)
                        gridTopTop.LockGrid();

                    if (gridTop.GridState != GridState.Filled)
                        gridTop.LockGrid();
                }
            }
        }
        
        [MethodTimer]
        private void SpawnObstacles(int layer)
        {
            var rotatedPlacables = new List<PlacableData>();

            foreach (var placable in _mapSettings.Placables)
                for (var i = 0; i < 360; i += placable.RotationDegreeStep)
                    rotatedPlacables.Add(new PlacableData(placable, i));

            var noise = _mapSettings.ObjectPlacementNoise.Generate(X, Z);

            foreach (var x in Enumerable.Range(0, X).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(0, Z).OrderBy(_ => UnityEngine.Random.value))
            {
                if (noise[x, z] * 100 < _mapSettings.ObjectPlacementThreshold) continue;

                var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();

                var grid = _grids[x, layer, z];

                foreach (var data in shuffledPlacables)
                {
                    if (!_gridHelper.IsPlacableSuitable(grid, data.Placable, data.Rotation)) continue;

                    SpawnObject(grid, data.Placable, GridLayer.Obstacle, data.Rotation);
                    break;
                }
            }
        }

        private void SpawnObject(GridElement pos, Placable placable, GridLayer gridLayer, float rotation)
        {
            if (!_gridHelper.IsPlacableSuitable(pos, placable, rotation))
                Debug.LogWarning("Trying to spawn an object that is not suitable");

            var placableObj = Instantiate(placable, _gridParent);
            placableObj.transform.position = pos.GetWorldPosition();
            placableObj.VisualsParent.eulerAngles = Vector3.up * rotation;


            _placedItems.Add(placableObj);

            foreach (var placableRequiredGrid in placable.RequiredGrids)
            {
                var checkedGridPos = pos.Position + _gridHelper.RotateObstacleVector(rotation, placableRequiredGrid);
                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];
                grid.FillGrid(placableObj, gridLayer);
            }

            foreach (var placableLockGrid in placable.LockGrids)
            {
                var checkedGridPos = pos.Position + _gridHelper.RotateObstacleVector(rotation, placableLockGrid);
                if (_gridHelper.IsPosOutsideOfGrids(checkedGridPos))
                    continue;

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];

                if (grid.GridState != GridState.Filled)
                    grid.LockGrid();
            }

            foreach (var placableNewGroundGrid in placable.NewGroundGrids)
            {
                var checkedGridPos = pos.Position + _gridHelper.RotateObstacleVector(rotation, placableNewGroundGrid);
                if (_gridHelper.IsPosOutsideOfGrids(checkedGridPos))
                    continue;

                var grid = _grids[checkedGridPos.x, checkedGridPos.y, checkedGridPos.z];

                if (grid.GridState is not (GridState.Locked or GridState.Filled))
                    grid.MakeGridCanBeFilledGround();
            }
        }

        private void DestroyGrid(GridElement grid)
        {
            var placable = grid.PlacedItem;

            if (grid.PlacedItem == null)
            {
                grid.FreeTheGrid();
                return;
            }

            foreach (var gridElement in _grids)
                if (gridElement.PlacedItem == placable)
                    grid.FreeTheGrid();

            _placedItems.Remove(placable);
            Destroy(placable.gameObject);
        }

        private void OnDrawGizmos()
        {
            if (_grids == null) return;

            if (_mapGizmos.HasFlag(MapGizmos.Paths))
            {
                foreach (var gridElements in _cachedPaths)
                {
                    for (var i = 0; i < gridElements.Count; i++)
                    {
                        var gridElement = gridElements[i];
                        if (i == 0)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere((Vector3)gridElement.Position * _offsetScaler, _gizmoRadius);
                            continue;
                        }

                        if (i == gridElements.Count - 1)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere((Vector3)gridElement.Position * _offsetScaler, _gizmoRadius);
                            continue;
                        }

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere((Vector3)gridElement.Position * _offsetScaler, _gizmoRadius);
                    }
                }
            }
            
            if (_mapGizmos.HasFlag(MapGizmos.Edges))
            {
                foreach (var edgeGroup in _edgeGroups)
                {
                    Gizmos.color = UnityEngine.Random.ColorHSV();
                    foreach (var edge in edgeGroup)
                    {
                        Gizmos.DrawSphere((Vector3)edge.GridElement.Position * _offsetScaler, _gizmoRadius);
                    }
                }
            }
            
            foreach (var grid in _grids)
            {
                foreach (MapGizmos flag in _mapGizmos.GetFlags())
                {
                    switch (flag)
                    {
                        case MapGizmos.None:
                            break;
                        case MapGizmos.Filled when grid.GridState == GridState.Filled:
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.Locked when grid.GridState == GridState.Locked:
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.CanBeFilled when grid.GridState == GridState.CanBeFilled:
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.PlacableGround when grid.GridLayer == GridLayer.CanPlacableGround:
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                    }
                }
            }
        }
    }
}