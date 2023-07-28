using MapGen.Placables;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(PlacableGrid))]
    public class PlacableGridEditor : UnityEditor.Editor
    {
        protected void OnEnable()
        {
            if(target == null) return;
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = true;
            placableGrid.OnValidate();
        }

        protected void OnDisable()
        {
            if(target == null) return;
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = false;
            placableGrid.OnValidate();
        }
    }
}