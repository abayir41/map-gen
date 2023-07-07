using System;
using System.Collections.Generic;
using MapGen.Placables;
using MapGen.Utilities;
using UnityEngine;

namespace LabGen.Placables
{
    public class MazeCubicGridPlacable : Placable
    {
        [SerializeField] private GameObject _wallVisual;
        [SerializeField] private int _wallVisualScale = 1;
        private Dictionary<MazeCubicPositions, List<GameObject>> _posVisualsDict = new();

        protected override void OnValidate()
        {
            if (useCubeRequiredGridStyle)
            {
                Debug.LogWarning($"You can't use '{nameof(useCubeRequiredGridStyle)}' in Placable that needs to be setup");
                useCubeRequiredGridStyle = false;
            }

            if (useVisualsAsRequiredGrids)
            {
                Debug.LogWarning($"You can't use '{nameof(useVisualsAsRequiredGrids)}' in Placable that needs to be setup");
                useVisualsAsRequiredGrids = false;
            }
            
            if (useVisualsAsNewGroundGrids)
            {
                Debug.LogWarning($"You can't use '{nameof(useVisualsAsNewGroundGrids)}' in Placable that needs to be setup");
                useVisualsAsNewGroundGrids = false;
            }

            if (useVisualAsShouldPlacedOnGroundGrids)
            {
                Debug.LogWarning($"You can't use '{nameof(useVisualAsShouldPlacedOnGroundGrids)}' in Placable that needs to be setup");
                useVisualAsShouldPlacedOnGroundGrids = false;
            }
            
            base.OnValidate();
        }

        public void SetupMazePlacable(int wallThickness, int wayThickness, int wallHeight)
        {
            InstantiateWalls(wallThickness, wayThickness, wallHeight);
            foreach (var (position, visuals) in _posVisualsDict)
            {
                foreach (var visual in visuals)
                {
                    visual.SetActive(false);
                }
            }
        }
        
        private void InstantiateWalls(int wallThickness, int wayThickness, int wallHeight)
        {
            var enums = Enum.GetValues(typeof(MazeCubicPositions));
            foreach (MazeCubicPositions position in enums)
            {
                _posVisualsDict.Add(position, new List<GameObject>());
            }
            
            for (var x = 0; x < wallThickness + wayThickness + wallThickness; x++)
            {
                for (var y = 0; y < wallThickness + wayThickness + wallThickness; y++)
                {
                    for (var h = 0; h < wallHeight; h++)
                    {
                        var position = FindMazeCubicPositionCorrespondingToPosition(new Vector2Int(x, y), wallThickness, wayThickness);
                        
                        if(position == MazeCubicPositions.Way) continue;
                        
                        var instantiated = Instantiate(_wallVisual, visualsParent);
                        instantiated.transform.localPosition = new Vector3(x, h, y) * _wallVisualScale;
                        instantiated.gameObject.name = position + "" + h;
                        _posVisualsDict[position].Add(instantiated);
                    }
                }
            }
        }

        private MazeCubicPositions FindMazeCubicPositionCorrespondingToPosition(Vector2Int pos, int wallThickness, int wayThickness)
        {
            if (pos.x >= 0 && pos.x < wallThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.BottomLeft;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Left;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.TopLeft;
                }

                throw new Exception("Invalid Position");
            }
            
            if (pos.x >= wallThickness && pos.x < wallThickness + wayThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.Bottom;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Way;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.Top;
                }
                
                throw new Exception("Invalid Position");
            }
            
            if (pos.x >= wallThickness + wayThickness && pos.x < wallThickness + wayThickness + wallThickness)
            {
                if (pos.y >= 0 && pos.y < wallThickness)
                {
                    return MazeCubicPositions.BottomRight;
                }

                if (pos.y >= wallThickness && pos.y < wallThickness + wayThickness)
                {
                    return MazeCubicPositions.Right;
                }

                if (pos.y >= wallThickness + wayThickness && pos.y < wallThickness + wayThickness + wallThickness)
                {
                    return MazeCubicPositions.TopRight;
                }
                
                throw new Exception("Invalid Position");
            }
            
            throw new Exception("Invalid Position");
        }

        public void OpenLeftWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.Left])
            {
                visual.SetActive(true);
            }
        }

        public void OpenTopLeftWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.TopLeft])
            {
                visual.SetActive(true);
            }
        }

        public void OpenBottomLeftWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.BottomLeft])
            {
                visual.SetActive(true);
            }
        }

        public void OpenRightWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.Right])
            {
                visual.SetActive(true);
            }
        }

        public void OpenTopRightWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.TopRight])
            {
                visual.SetActive(true);
            }
        }

        public void OpenBottomRightWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.BottomRight])
            {
                visual.SetActive(true);
            }
        }

        public void OpenTopWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.Top])
            {
                visual.SetActive(true);
            }
        }

        public void OpenBottomWall()
        {
            foreach (var visual in _posVisualsDict[MazeCubicPositions.Bottom])
            {
                visual.SetActive(true);
            }
        }
    }
}