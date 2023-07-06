using MapGen.Random;
using UnityEngine;

namespace LabGen.Labyrinth
{
    
    [CreateAssetMenu(fileName = "Labyrinth Settings", menuName = "LabGen/Labyrinth/Settings", order = 0)]
    public class LabyrinthSettings : ScriptableObject
    {
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private RandomSettings _randomSettings;
        [SerializeField] private MazeGridPlacable _mazeGridPlacable;
        
        public MazeGridPlacable MazeGridPlacable => _mazeGridPlacable;
        public RandomSettings RandomSettings => _randomSettings;
        public Vector2Int MapSize 
        {
            get
            {
                var mapSize = new Vector2Int(Mathf.Clamp(_mapSize.x, 5, 200),Mathf.Clamp(_mapSize.y, 5, 200));
                _mapSize = mapSize;
                return mapSize;
            }
        }
    }
}