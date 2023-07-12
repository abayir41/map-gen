using UnityEngine;

namespace MapGen.Map
{
    [CreateAssetMenu(fileName = "World Settings", menuName = "MapGen/World/Settings", order = 0)]
    public class WorldSettings : ScriptableObject
    {
        private const int MAX_SIZE = 200;
        private const int MIN_SIZE = 5;
        
        [Header("Map Settings")]
        [SerializeField] private Vector3Int _mapSize;

        public Vector3Int MapSize => _mapSize;

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
            
            if (_mapSize.z > MAX_SIZE)
            {
                Debug.LogWarning($"Z value exceed Max({MAX_SIZE}), setting to -> {MAX_SIZE}");
            }
            else if (_mapSize.z < 5)
            {
                Debug.LogWarning($"Z value exceed Min({MIN_SIZE}), setting to -> {MIN_SIZE}");
            }

            var x = Mathf.Clamp(_mapSize.x, MIN_SIZE, MAX_SIZE);
            var y = Mathf.Clamp(_mapSize.y, MIN_SIZE, MAX_SIZE);
            var z = Mathf.Clamp(_mapSize.z, MIN_SIZE, MAX_SIZE);
            _mapSize = new Vector3Int(x, y, z);
        }
    }
}