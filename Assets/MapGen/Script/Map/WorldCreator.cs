using System.Collections.Generic;
using MapGen.GridSystem;
using MapGen.Map.Brushes;
using MapGen.Placables;
using UnityEngine;
using Grid = MapGen.GridSystem.Grid;

namespace MapGen.Map
{
    public class WorldCreator : MonoBehaviour
    {
        public static WorldCreator Instance { get; private set; }
        
        [SerializeField] private WorldSettings _worldSettings;
        [SerializeField] private Transform _gridPrefabsParent;
        [SerializeField] private BrushSelector _brushSelector;

        public Grid Grid { get; private set; }

        private void Awake()
        {
            Instance = this;
            Grid = new Grid();
        }

        public void PaintTheBrush(List<Vector3Int> selectedCells)
        {
            _brushSelector.CurrentBrush.Paint(selectedCells, Grid);

        }
        
        private void DestroyPlacedPlacables(List<Vector3Int> selectedCells)
        {
            foreach (var selectedCell in selectedCells)
            {
                if(Grid.IsCellExist(selectedCell, out var cell) && cell.Item != null)
                {
                    DestroyItem(cell.Item);
                }
            }
        }
        
        public void DestroyItem(Placable placable)
        {
            Grid.DeleteItem(placable);
            DestroyPlacable(placable);
        }

        private void DestroyPlacable(Placable item)
        {
            Destroy(item.gameObject);
        }

        

        public Placable SpawnObject(Vector3Int pos, Placable placable, CellLayer cellLayer, float rotation, string objName = null)
        {
            var instantiatedPlacable = Instantiate(placable, _gridPrefabsParent);
            instantiatedPlacable.InitializePlacable(pos);
            instantiatedPlacable.transform.position = Grid.CellPositionToRealWorld(pos);

            if (objName != null)
                instantiatedPlacable.name = objName;
                
            if (instantiatedPlacable.Rotatable)
            {
                instantiatedPlacable.Rotate(rotation);
            }
            
            Grid.AddItem(instantiatedPlacable, pos, rotation, cellLayer);
         
            
            return instantiatedPlacable;
        }
    }
}