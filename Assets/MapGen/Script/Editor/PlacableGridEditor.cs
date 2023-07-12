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
        protected void OnEnable()
        {
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = true;
            placableGrid.OnValidate();
        }

        protected void OnDisable()
        {
            var placableGrid = (PlacableGrid) target;
            placableGrid.DrawGizmo = false;
            placableGrid.OnValidate();
        }
    }
}