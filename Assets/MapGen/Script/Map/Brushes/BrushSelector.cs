using System;
using System.Collections.Generic;
using MapGen.Map.Brushes.Eraser;
using MapGen.Map.Brushes.NormalMap;
using MapGen.Map.Brushes.ObstacleSpawner;
using TMPro;
using UnityEngine;

namespace MapGen.Map.Brushes
{
    public class BrushSelector : MonoBehaviour
    {
        [SerializeField] private FpsState _fpsState;
        [SerializeField] private TextMeshProUGUI brushName;

        [SerializeField] private GroundBrush.GroundBrush _groundBrush;
        [SerializeField] private Labyrinth.LabyrinthBrush _labyrinthBrush;
        [SerializeField] private MapBrush _mapBrush;
        [SerializeField] private ObstaclesBrush _obstaclesBrush;


        public IBrush CurrentBrush { get; private set; }

        private List<IBrush> _brushes;
        private int _brushIndex;

        private void Awake()
        {
            _brushIndex = 0;
            _brushes = new List<IBrush>();
            _brushes.Add(_groundBrush);
            _brushes.Add(_labyrinthBrush);
            _brushes.Add(_mapBrush);
            _brushes.Add(new EraserBrush());
            _brushes.Add(new CharPositionBrush(_fpsState));
            _brushes.Add(_obstaclesBrush);

            CurrentBrush = _groundBrush;
            brushName.text = "Brush: " + CurrentBrush.BrushName;
        }

        public IBrush NextBrush()
        {
            _brushIndex++;
            if (_brushIndex >= _brushes.Count)
            {
                _brushIndex = 0;
            }

            CurrentBrush = _brushes[_brushIndex];
            brushName.text = "Brush: " + CurrentBrush.BrushName;
            return CurrentBrush;
        }

        public IBrush PreviousBrush()
        {
            _brushIndex--;
            if (_brushIndex <= -1)
            {
                _brushIndex = _brushes.Count - 1;
            }
            
            CurrentBrush = _brushes[_brushIndex];
            brushName.text = "Brush: " + CurrentBrush.BrushName;
            return CurrentBrush;
        }
    }
}