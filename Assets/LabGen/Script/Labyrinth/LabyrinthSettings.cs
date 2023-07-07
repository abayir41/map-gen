using System;
using LabGen.Placables;
using MapGen.Random;
using UnityEngine;

namespace LabGen.Labyrinth
{
    
    [CreateAssetMenu(fileName = "Labyrinth Settings", menuName = "LabGen/Labyrinth/Settings", order = 0)]
    public class LabyrinthSettings : ScriptableObject
    {
        private const int MAX_SIZE = 200;
        private const int MIN_SIZE = 5;
        
        
        [Header("Labyrinth Settings")]
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private RandomSettings _randomSettings;
        [SerializeField] private int _wallThickness;
        [SerializeField] private int _wayThickness;
        [SerializeField] private int _wallHeight;
        [SerializeField] private MazeCubicGridPlacable _mazeCubicGridPlacable;
        
        public MazeCubicGridPlacable MazeCubicGridPlacable => _mazeCubicGridPlacable;
        public RandomSettings RandomSettings => _randomSettings;
        public Vector2Int MapSize => _mapSize;
        public int WallThickness => _wallThickness;
        public int WayThickness => _wayThickness;
        public int WallHeight => _wallHeight;

        private void OnValidate()
        {
            if (_mapSize.x > MAX_SIZE)
            {
                Debug.LogWarning($"X value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.x < MIN_SIZE)
            {
                Debug.LogWarning($"X value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }
            
            if (_mapSize.y > MAX_SIZE)
            {
                Debug.LogWarning($"Y value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.y < 5)
            {
                Debug.LogWarning($"Y value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }
            
            _mapSize = new Vector2Int(Mathf.Clamp(_mapSize.x, MIN_SIZE, MAX_SIZE),Mathf.Clamp(_mapSize.y, MIN_SIZE, MAX_SIZE));
        }
    }
}