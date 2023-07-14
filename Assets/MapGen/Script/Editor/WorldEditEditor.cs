using MapGen.Map.MapEdit;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(WorldEdit))]
    public class WorldEditEditor : SettingDrawer<WorldEdit>
    {
        private UnityEditor.Editor _cachedEditor;
        private bool _foldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(Self.CurrentSelectCubicBrush, ref _foldout, ref _cachedEditor);
        }
    }
}