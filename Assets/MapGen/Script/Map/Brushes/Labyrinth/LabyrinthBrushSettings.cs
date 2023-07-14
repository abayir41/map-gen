using MapGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace MapGen.Map.Brushes.Labyrinth
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
        [SerializeField] private Placable _ground;
        public const int GROUND_ROTATION = 0;
        public const int GROUND_Y_LEVEL = 0;
        public const int LABYRINTH_START_Y_LEVEL = 1;

        public Placable Ground => _ground;
        public Placable MazeCubicGridPlacable => _mazeCubicGridPlacable;
        public RandomSettings RandomSettings => _randomSettings;
        public int WallThickness => _wallThickness;
        public int WayThickness => _wayThickness;
        public int WallHeight => _wallHeight;
    }
}