using MapGen.Placables;
using UnityEditor;

namespace MapGen.Editor
{
    
    [CustomEditor(typeof(Placable))]
    public class PlacableEditor : UnityEditor.Editor
    {
        protected void OnEnable()
        {
            if(target == null) return;
            var placableGrid = (Placable) target;
            placableGrid.DrawGizmo = true;
            placableGrid.OnValidate();
        }

        protected void OnDisable()
        {
            if(target == null) return;
            var placableGrid = (Placable) target;
            placableGrid.DrawGizmo = false;
            placableGrid.OnValidate();
        }
    }
}