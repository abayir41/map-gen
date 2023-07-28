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
        [SerializeField] private List<Brush> _brushes;


        public Brush CurrentBrush => _brushes[_brushIndex];
        public BrushArea CurrentBrushArea
        {
            get
            {
                if (CurrentBrush.CurrentBrushArea == null)
                    CurrentBrush.CurrentBrushArea = CurrentBrush.BrushAreas[_brushAreaIndex];
                
                return CurrentBrush.CurrentBrushArea;
            }
        }

        private int _brushIndex;
        private int _brushAreaIndex;

        private void Awake()
        {
            Instance = this;
            
            _brushIndex = 0;
        }

        public void NextBrush()
        {
            _brushAreaIndex = 0;
            _brushIndex++;
            if (_brushIndex >= _brushes.Count)
            {
                _brushIndex = 0;
            }
            
            CurrentBrush.CurrentBrushArea = CurrentBrush.BrushAreas[_brushAreaIndex];
        }

        public void PreviousBrush()
        {
            _brushAreaIndex = 0;
            _brushIndex--;
            if (_brushIndex <= -1)
            {
                _brushIndex = _brushes.Count - 1;
            }
            
            CurrentBrush.CurrentBrushArea = CurrentBrush.BrushAreas[_brushAreaIndex];
        }

        public void NextBrushArea()
        {
            _brushAreaIndex++;
            if (_brushAreaIndex >= CurrentBrush.BrushAreas.Count)
            {
                _brushAreaIndex = 0;
            }
            
            CurrentBrush.CurrentBrushArea = CurrentBrush.BrushAreas[_brushAreaIndex];
        }
        
        public void PreviousBrushArea()
        {
            _brushAreaIndex--;
            if (_brushAreaIndex <= -1)
            {
                _brushAreaIndex = CurrentBrush.BrushAreas.Count - 1;
            }
            
            CurrentBrush.CurrentBrushArea = CurrentBrush.BrushAreas[_brushAreaIndex];
        }
    }
}