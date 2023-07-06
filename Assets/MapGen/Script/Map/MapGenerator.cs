using System.Collections.Generic;
using System.Linq;
using MapGen.GridSystem;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using Plugins.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MapGen.Map
{
    public class MapGenerator : MonoBehaviour
    {
        ////////////
        /// Editor
        ///////////
        [SerializeField] [HideInInspector] public bool MapSettingsFoldout;


        [SerializeField] private MapSettings _mapSettings;
        [SerializeField] private Transform _gridParent;
        [SerializeField] private bool _editorAutoGenerateMap;


        [Header("Gizmo Settings")] [SerializeField]
        private bool _drawGizmo = true;

        [SerializeField] private bool _showFilled = true;
        [SerializeField] private bool _showLocked = true;
        [SerializeField] private bool _showCanBeFilled = true;
        [SerializeField] private bool _showPlacableGround = true;
        [SerializeField] private bool _showPaths = true;
        [SerializeField] private float _gizmoRadius = 0.25f;
        [SerializeField] private float _offsetScaler = 1f;
        private List<List<GridElement>> _cachedPaths = new();

        public MapSettings MapSettings => _mapSettings;
        private int X => _mapSettings.X;
        private int Y => _mapSettings.Y;
        private int Z => _mapSettings.Z;

        private GridHelper _gridHelper;
        private GridElement[,,] _grids;
        private List<Placable> _placedItems = new();


        /// <summary>
        /// This function called from Editor, Look at: "MapGeneratorEditor.cs"
        /// </summary>
        public void GenerateMapAuto()
        {
            if (!Application.isPlaying) return;
            if (!_editorAutoGenerateMap) return;

            GenerateMap();
        }

        /// <summary>
        /// This function called from Editor, Look at: "MapGeneratorEditor.cs"
        /// </summary>
        public void GenerateMap()
        {
            if (!Application.isPlaying) return;

            SetupGeneration();
            Generate();
        }

        private void SetupGeneration()
        {
            foreach (var placedItem in _placedItems) Destroy(placedItem.gameObject);

            _placedItems.Clear();
            _cachedPaths.Clear();
            
            _grids = new GridElement[X, Y, Z];
            _gridHelper = new GridHelper(_grids, _mapSettings);
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var y = 0; y < _grids.GetLength(1); y++)
            for (var z = 0; z < _grids.GetLength(2); z++)
                _grids[x, y, z] = new GridElement(x, y, z);

            UnityEngine.Random.InitState(_mapSettings.RandomSettings.GetSeed());
        }

        private void Generate()
        {
            Debug.Log("Generation Started");
            CreateGround();
            MakeTunnels(_mapSettings.TunnelYLevel);
            CreateWall();
            MakeBanAreaOfSurface();

            for (var height = 0; height < _mapSettings.ObstaclesMaxHeight; height++)
            for (var i = 0; i < _mapSettings.IterationAmount; i++)
                SpawnObstacles(height);
        }

        private void CreateGround()
        {
            var groundNoise = _mapSettings.GroundPlacementNoise.Generate(X, Z);

            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var grid = _grids[x, 0, z];
                grid.MakeGridCanBeFilledGround();
                SpawnObject(grid, _mapSettings.Ground, GridLayer.Ground, 0);
            }

            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var height = groundNoise[x, z] * _mapSettings.GroundHeightFactor - _mapSettings.GroundMoveDownFactor;

                for (var y = 1; y < height; y++)
                {
                    if (groundNoise[x, z] * 100 < height) break;
                    var grid = _grids[x, y, z];
                    grid.MakeGridCanBeFilledGround();
                    SpawnObject(grid, _mapSettings.Ground, GridLayer.Ground, 0);
                }
            }
        }

        private void MakeTunnels(int layer)
        {
            var nodes = CreateTunnelNodes(layer);
            var pathFinder = CreateTunnelNodeGraph(nodes, layer);
            var tunnelPaths = FindAllEdgeToEdgePaths(nodes, pathFinder);

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

                    if (grid.PlacedItem == null) continue;
                    
                    _placedItems.Remove(ground);
                    Destroy(ground.gameObject);
                }

                if (_gridHelper.IsPlacableSuitable(pathPoint, _mapSettings.TunnelBrush, rotationDegree))
                    SpawnObject(pathPoint, _mapSettings.TunnelBrush, GridLayer.Obstacle, rotationDegree);
            }
            
            _cachedPaths.Add(path);
        }

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
                        _mapSettings.TunnelMinLength) continue;

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

            var newPath =
                BresenhamLineAlgorithm.DrawLine(new Vector2Int(start.x, start.z), new Vector2Int(end.x, end.z));
            var result = newPath.ConvertAll(input => _grids[input.x, yLayer, input.y]);
            return result;
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
                SpawnObject(grid, _mapSettings.Wall, GridLayer.Obstacle, 0);
            }
        }

        private void MakeBanAreaOfSurface()
        {
            for (var x = 0; x < _grids.GetLength(0); x++)
            for (var z = 0; z < _grids.GetLength(2); z++)
            {
                var gridTopTop = _grids[x, _grids.GetLength(1) - 1, z];
                var gridTop = _grids[x, _grids.GetLength(1) - 2, z];

                if (gridTopTop.GridState != GridState.Filled)
                    gridTopTop.LockGrid();

                if (gridTop.GridState != GridState.Filled)
                    gridTop.LockGrid();
            }
        }

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
            if (!_drawGizmo) return;

            if (_showPaths)
                foreach (var gridElements in _cachedPaths)
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

            foreach (var grid in _grids)
            {
                var shouldSkip = true;
                switch (grid.GridState)
                {
                    case GridState.Filled when _showFilled:
                    case GridState.Locked when _showLocked:
                    case GridState.CanBeFilled when _showCanBeFilled:
                        shouldSkip = false;
                        break;
                }

                switch (grid.GridLayer)
                {
                    case GridLayer.CanPlacableGround when _showPlacableGround:
                        shouldSkip = false;
                        break;
                }

                if(shouldSkip) continue;
                
                var color = Color.black;
                color = grid.GridState switch
                {
                    GridState.Filled => Color.blue,
                    GridState.CanBeFilled => Color.green,
                    GridState.Locked => Color.red,
                    _ => color
                };

                color = grid.GridLayer switch
                {
                    GridLayer.CanPlacableGround => Color.yellow,
                    _ => color
                };

                Gizmos.color = color;
                Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
            }
        }
    }
}