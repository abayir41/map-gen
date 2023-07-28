using System;
using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.Brushes.BrushAreas;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map.Brushes
{
    
    [CreateAssetMenu(fileName = "Obstacle Brush", menuName = "MapGen/Brushes/Obstacle/Brush", order = 0)]
    public class PlacableSpawner : Brush
    {
        [SerializeField] private List<PlacableArea> _placableAreas;

        public override List<BrushArea> BrushAreas => _placableAreas.ConvertAll(input => (BrushArea) input);
        public override string BrushName => "Placable";
        
        public override void Paint(List<Vector3Int> selectedCells, Grid grid, Vector3Int startPoint)
        {
            var brushArea = (PlacableArea) CurrentBrushArea;
            if (!WorldCreator.Instance.Grid.IsPlacableSuitable(startPoint, brushArea.Placable, 0)) return;

            WorldCreator.Instance.SpawnObject(startPoint, brushArea.Placable, CellLayer.Obstacle, 0);
        }
    }
}