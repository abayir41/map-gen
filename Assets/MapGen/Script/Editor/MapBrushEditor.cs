using MapGen.Map.Brushes.NormalMap;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(MapBrush))]
    public class MapBrushEditor : SettingDrawer<MapBrush>
    {
        private UnityEditor.Editor _cachedEditor;
        private bool _foldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(Self.MapBrushSettings, ref _foldout, ref _cachedEditor);
        }
    }
}