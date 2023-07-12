using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushHelper;
using MapGen.Placables;
using MapGen.TunnelSystem;
using MapGen.Utilities;
using Plugins.Utilities;
using UnityEngine;
using Weaver;

namespace MapGen.Map.Brushes.NormalMap
{
    [CreateAssetMenu(fileName = "Map Brush", menuName = "MapGen/Brushes//Normal Map/Brush", order = 0)]
    public class MapBrush : Brush
    {
        [SerializeField] private MapBrushSettings _mapBrushSettings;
        private MapBrushCellsHelper _selectedCells;
        private Grid _grid;

        public MapBrushSettings MapBrushSettings => _mapBrushSettings;

        public override List<Placable> Paint(List<GridCell> selectedCells, Grid grid)
        {
            _grid = grid;
            _selectedCells = new MapBrushCellsHelper(selectedCells, grid, _mapBrushSettings);
            return GenerateMap();
        }
        
        [MethodTimer]
        public List<Placable> GenerateMap()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Method Should be called when game is playing, It can't be called from Editor");
                return null;
            }

            SetupGeneration();
            return Generate();
        }

        private void SetupGeneration()
        {
            SetRandomSeed();
        }

        private void SetRandomSeed()
        {
            UnityEngine.Random.InitState(_mapBrushSettings.RandomSettings.GetSeed());
        }
        
        private List<Placable> Generate()
        {
            Debug.Log("Generation Started");
            
            var result = new List<Placable>();
            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Ground))
            {
                var placables = CreateGround();
                result.AddRange(placables);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Mountains))
            {
                var placables = CreateMountains();
                result.AddRange(placables);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Tunnels))
            {
                var placables = MakeTunnels(_mapBrushSettings.TunnelYLevel);
                if(placables != null) 
                    result.AddRange(placables);
            }

            if (_mapBrushSettings.MapParts.HasFlag(MapParts.Obstacles))
            {
                for (var height = 0; height < _mapBrushSettings.ObstaclesMaxHeight; height++)
                {
                    for (var i = 0; i < _mapBrushSettings.IterationAmount; i++)
                    {
                        var placables = SpawnObstacles(height);
                        result.AddRange(placables);
                    }
                }
            }

            return result;
        }

        [MethodTimer]
        private List<Placable> CreateGround()
        {
            var result = new List<Placable>();
            var groundCells = _selectedCells.GetYAxisOfGrid(MapBrushSettings.GROUND_Y_LEVEL);
            foreach (var selectedCell in groundCells)
            {
                selectedCell.MakeCellCanBeFilledGround();
                var placable =  WorldCreator.Instance.SpawnObject(selectedCell, _mapBrushSettings.Ground, CellLayer.Ground, MapBrushSettings.GROUND_ROTATION);
                result.Add(placable);
            }

            return result;
        }

        [MethodTimer]
        private List<Placable> CreateMountains()
        {
            var mountains = _mapBrushSettings.GroundPlacementNoise.Generate(_grid.MaxX, _grid.MaxZ);
            var result = new List<Placable>();

            foreach (var selectedCell in _selectedCells.Cells)
            {
                var selectedCellPos = selectedCell.Position;
                var height = mountains[selectedCellPos.x, selectedCellPos.z] * _mapBrushSettings.GroundHeightFactor -
                             _mapBrushSettings.GroundMoveDownFactor;

                for (var y = MapBrushSettings.MOUNTAIN_Y_START_LEVEL; y < height; y++)
                {
                    if (ZeroOneIntervalToPercent(mountains[selectedCellPos.x, selectedCellPos.z]) < height) break;
                    var targetPos = new Vector3Int(selectedCell.Position.x, y, selectedCell.Position.z);
                    var cell = _selectedCells.Cells.Find(gridCell => gridCell.Position == targetPos);
                    cell.MakeCellCanBeFilledGround();
                    var placable = WorldCreator.Instance.SpawnObject(cell, _mapBrushSettings.Ground, CellLayer.Ground, MapBrushSettings.GROUND_ROTATION);
                    result.Add(placable);
                }
            }
            return result;
        }

        private float ZeroOneIntervalToPercent(float zeroOneInterval)
        {
            return zeroOneInterval * 100;
        }

        [MethodTimer]
        private List<Placable> MakeTunnels(int layer)
        {
            var nodes = CreateTunnelNodes(layer);
            var pathFinder = CreateTunnelNodeGraph(nodes, layer);
            var edges = nodes.Where(node => node.NodeState == NodeState.EdgeGround).ToList();
            var edgeGroups = GroupEdgeNodes(edges);
            var tunnelPaths = FindAllEdgeToEdgePathsFromGroups(edgeGroups, nodes, pathFinder);

            if (tunnelPaths.Count == 0) return null;

            var pathsAsGridCells = tunnelPaths.ConvertAll(input => input.ConvertAll(node => node.GridCell));
            var lenghtFilteredTunnels = _selectedCells.FilterByHeight(pathsAsGridCells);
            var fixedTunnelPaths = lenghtFilteredTunnels.ConvertAll(FixThePath);
            var heightFilteredTunnels = _selectedCells.FilterPathsByLenght(fixedTunnelPaths);
            var orderedTunnels = heightFilteredTunnels.OrderByDescending(_selectedCells.FindHeightAverageOfPath).ToList();

            var cachedPaths = new List<List<GridCell>>();
            var result = new List<Placable>();
            foreach (var path in orderedTunnels)
            {
                if (_selectedCells.CanTunnelSpawnable(path, cachedPaths))
                {
                    var placable = CreateTunnel(path);
                    cachedPaths.Add(path);
                    result.AddRange(placable);
                }
            }

            return result;
        }

        [MethodTimer]
        private List<Placable> CreateTunnel(List<GridCell> path)
        {
            var start = path.First();
            var end = path.Last();
            var direction = end.Position - start.Position;
            var direction2D = new Vector2Int(direction.x, direction.z);
            var rotationDegree = Mathf.Atan2(direction2D.y, direction2D.x) / Mathf.PI * 180;
            rotationDegree *= -1;

            var result = new List<Placable>();

            foreach (var pathPoint in path)
            {
                foreach (var tunnelBrushDestroyPoint in _mapBrushSettings.TunnelBrush.DestroyPoints.CellPositions)
                {
                    var pos = pathPoint.Position +
                              GridHelper.RotateObstacleVector(rotationDegree, tunnelBrushDestroyPoint);

                    if (_selectedCells.IsPosOutsideOfGrid(pos)) continue;

                    var cell = _selectedCells.GetCell(new Vector3Int(pos.x, pos.y, pos.z));
                    if (cell.PlacedItem is TunnelBrush) continue;

                    WorldCreator.Instance.DestroyItem(cell.PlacedItem);
                }

                if (_selectedCells.IsPlacableSuitable(pathPoint, _mapBrushSettings.TunnelBrush, rotationDegree))
                {
                    var placable = WorldCreator.Instance.SpawnObject(pathPoint, _mapBrushSettings.TunnelBrush, CellLayer.Obstacle, rotationDegree);
                    result.Add(placable);
                }
            }

            return result;
        }

        [MethodTimer]
        private List<Node> CreateTunnelNodes(int layer)
        {
            var nodes = new List<Node>();

            var yFilteredSelected = _selectedCells.GetYAxisOfGrid(layer);
            foreach (var selectedCell in yFilteredSelected)
            {
                if (selectedCell.CellState == CellState.Filled && _selectedCells.IsEdgeGroundYDimensionCheck(selectedCell))
                {
                    nodes.Add(new Node(nodes.Count, NodeState.EdgeGround, selectedCell));
                    continue;
                }

                if (selectedCell.CellState == CellState.Filled)
                {
                    nodes.Add(new Node(nodes.Count, NodeState.Ground, selectedCell));
                }
            }
            return nodes;
        }

        [MethodTimer]
        private PathFinder CreateTunnelNodeGraph(List<Node> nodes, int layer)
        {
            var pathFinder = new PathFinder(nodes.Count);

            for (var x = _selectedCells.MinX; x < _selectedCells.MaxX - 1; x++)
            for (var z = _selectedCells.MinZ; z < _selectedCells.MaxZ - 1; z++)
            {
                var cell = _selectedCells.GetCell(new Vector3Int(x, layer, z));

                if (nodes.All(node => node.GridCell != cell))
                    continue;

                var nodeGrid = nodes.First(node => node.GridCell == cell);
                var right = _selectedCells.GetCell(new Vector3Int(x + 1, layer, z));
                var bottom = _selectedCells.GetCell(new Vector3Int(x, layer, z + 1));

                if (nodes.Any(node => node.GridCell == right))
                {
                    var rightGrid = nodes.First(node => node.GridCell == right);
                    pathFinder.AddEdge(rightGrid.ID, nodeGrid.ID);
                }

                if (nodes.Any(node => node.GridCell == bottom))
                {
                    var bottomGrid = nodes.First(node => node.GridCell == bottom);
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
                        node.GridCell.Position == start.GridCell.Position + new Vector3Int(xAxis, 0, yAxis));

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
                        if ((edgeNodeA.GridCell.Position - edgeNodeB.GridCell.Position).sqrMagnitude <
                            _mapBrushSettings.TunnelMinLength) return;

                        var path = pathFinder.FindShortestPath(edgeNodeA.ID, edgeNodeB.ID);

                        if (path == null) return;

                        var pathAsNodes = path.ConvertAll(input => allNodes.First(node => node.ID == input));
                        paths.Add(pathAsNodes);
                    });
                });
            });
            return paths;
        }

        private List<GridCell> FixThePath(List<GridCell> path)
        {
            var start = path.First().Position;
            var end = path.Last().Position;
            var yLayer = start.y;

            var newPath =
                BresenhamLineAlgorithm.DrawLine(new Vector2Int(start.x, start.z), new Vector2Int(end.x, end.z));
            var result = newPath.ConvertAll(input => _selectedCells.GetCell(new Vector3Int(input.x, yLayer, input.y)));
            return result;
        }


        [MethodTimer]
        private List<Placable> SpawnObstacles(int layer)
        {
            var rotatedPlacables = new List<PlacableData>();

            foreach (var placable in _mapBrushSettings.Placables)
            {
                if(!placable.Rotatable) continue;
                
                for (var i = 0; i < _mapBrushSettings.MaxObstacleRotation; i += placable.RotationDegreeStep)
                {
                    rotatedPlacables.Add(new PlacableData(placable, i));
                }
            }
                

            var noise = _mapBrushSettings.ObjectPlacementNoise.Generate(_grid.MaxX, _grid.MaxZ);

            var result = new List<Placable>();
            
            
            foreach (var x in Enumerable.Range(_selectedCells.MinX, _selectedCells.XWidth).OrderBy(_ => UnityEngine.Random.value))
            foreach (var z in Enumerable.Range(_selectedCells.MinZ, _selectedCells.ZWidth).OrderBy(_ => UnityEngine.Random.value))
            {
                if (ZeroOneIntervalToPercent(noise[x, z]) < _mapBrushSettings.ObjectPlacementThreshold) continue;

                var shuffledPlacables = rotatedPlacables.GetRandomAmountAndShuffled();

                var cell = _selectedCells.GetCell(new Vector3Int(x, layer, z));

                foreach (var data in shuffledPlacables)
                {
                    if (!_selectedCells.IsPlacableSuitable(cell, data.Placable, data.Rotation)) continue;

                    var placable = WorldCreator.Instance.SpawnObject(cell, data.Placable, CellLayer.Obstacle, data.Rotation);
                    result.Add(placable);
                    break;
                }
            }

            return result;
        }
        
        /*private void OnDrawGizmos()
        {
            if(!_drawGizmos) return;
            if (_cells == null) return;

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
                        Gizmos.DrawSphere((Vector3)edge.GridCell.Position * _offsetScaler, _gizmoRadius);
                    }
                }
            }
            
            foreach (var grid in _cells)
            {
                foreach (MapGizmos flag in _mapGizmos.GetFlags())
                {
                    switch (flag)
                    {
                        case MapGizmos.None:
                            break;
                        case MapGizmos.Filled when grid.CellState == CellState.Filled:
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.Locked when grid.CellState == CellState.Locked:
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.CanBeFilled when grid.CellState == CellState.CanBeFilled:
                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                        case MapGizmos.PlacableGround when grid.CellLayer == CellLayer.CanPlacableGround:
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawSphere((Vector3)grid.Position * _offsetScaler, _gizmoRadius);
                            break;
                    }
                }
            }
        }*/
    }
}