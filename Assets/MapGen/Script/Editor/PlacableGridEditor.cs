using System;
using MapGen.Map;
using MapGen.Placables;
using UnityEditor;
using UnityEngine;

namespace MapGen.Editor
{
    [CustomEditor(typeof(PlacableGrid))]
    public class PlacableGridEditor : UnityEditor.Editor
    {
        protected virtual void OnEnable()
        {
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = true;
            placableGrid.OnValidate();
        }

        protected virtual void OnDisable()
        {
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = false;
            placableGrid.OnValidate();
        }
    }
    
    [CustomEditor(typeof(PlacableNewGroundGrid))]
    public class PlacableNewGroundEditor : PlacableGridEditor
    {
        
    }
    
    [CustomEditor(typeof(PlacableNormalGrid))]
    public class PlacableNormalEditor : PlacableGridEditor
    {
        
    }
    
    [CustomEditor(typeof(PlacableShouldPlaceOnGroundGrid))]
    public class PlacableShouldPlaceOnGroundEditor : PlacableGridEditor
    {
        
    }
    
    

}