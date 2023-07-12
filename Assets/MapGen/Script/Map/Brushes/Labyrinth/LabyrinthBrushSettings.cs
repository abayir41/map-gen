using System;
using LabGen.Placables;
using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace LabGen.Labyrinth
{
    
    [CreateAssetMenu(fileName = "Labyrinth Brush Settings", menuName = "MapGen/Brushes/Labyrinth/Brush Settings", order = 0)]
    public class LabyrinthBrushSettings : ScriptableObject
    {
        [Header("Labyrinth Settings")]
        [SerializeField] private RandomSettings _randomSettings;
        [SerializeField] private int _wallThickness;
        [SerializeField] private int _wayThickness;
        [SerializeField] private int _wallHeight;
        [SerializeField] private Placable _mazeCubicGridPlacable;
        public const int GROUND_ROTATION = 0;

        
        public Placable MazeCubicGridPlacable => _mazeCubicGridPlacable;
        public RandomSettings RandomSettings => _randomSettings;
        public int WallThickness => _wallThickness;
        public int WayThickness => _wayThickness;
        public int WallHeight => _wallHeight;
    }
}