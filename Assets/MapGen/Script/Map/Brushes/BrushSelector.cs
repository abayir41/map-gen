using System;
using System.Collections.Generic;
using MapGen.Map.Brushes.BrushAreas;
using MapGen.Map.Brushes.Eraser;
using MapGen.Map.Brushes.Mountains;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Map.Brushes.ObstacleSpawner;
using TMPro;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    public class BrushSelector : MonoBehaviour
    {
        public static BrushSelector Instance { get; private set; }
        
        [SerializeField] private FpsState _fpsState;
        [SerializeField] private TextMeshProUGUI brushName;

        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private Labyrinth.LabyrinthBrush _labyrinthBrush;
        [SerializeField] private MapBrush _mapBrush;
        [SerializeField] private ObstaclesBrush _obstaclesBrush;
        [SerializeField] private MountainBrush _mountainBrush;
        [SerializeField] private EraserBrush _eraserBrush;
        [SerializeField] private CharPositionBrush _charPositionBrush;

        public IBrush CurrentBrush => _brushes[_brushIndex];
        public IBrushArea CurrentBrushArea => CurrentBrush.BrushAreas[_brushAreaIndex];

        private List<IBrush> _brushes;
        private int _brushIndex;
        private int _brushAreaIndex;

        private void Awake()
        {
            Instance = this;
            
            _brushIndex = 0;
            _brushes = new List<IBrush>();
            _brushes.Add(_groundBrush);
            _brushes.Add(_labyrinthBrush);
            _brushes.Add(_mapBrush);
            _brushes.Add(_eraserBrush);
            _brushes.Add(_charPositionBrush);
            _brushes.Add(_obstaclesBrush);
            _brushes.Add(_mountainBrush);

            brushName.text = "Brush: " + CurrentBrush.BrushName;
        }

        public void NextBrush()
        {
            _brushAreaIndex = 0;
            _brushIndex++;
            if (_brushIndex >= _brushes.Count)
            {
                _brushIndex = 0;
            }

            brushName.text = "Brush: " + CurrentBrush.BrushName;
        }

        public void PreviousBrush()
        {
            _brushAreaIndex = 0;
            _brushIndex--;
            if (_brushIndex <= -1)
            {
                _brushIndex = _brushes.Count - 1;
            }
            
            brushName.text = "Brush: " + CurrentBrush.BrushName;
        }

        public void NextBrushArea()
        {
            _brushAreaIndex++;
            if (_brushAreaIndex >= CurrentBrush.BrushAreas.Count)
            {
                _brushAreaIndex = 0;
            }
        }
        
        public void PreviousBrushArea()
        {
            _brushAreaIndex--;
            if (_brushAreaIndex <= -1)
            {
                _brushAreaIndex = CurrentBrush.BrushAreas.Count - 1;
            }
        }
    }
}