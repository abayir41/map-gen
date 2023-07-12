using MapGen.Map;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(WorldCreator))]
    public class WorldCreatorEditor : SettingDrawer<WorldCreator>
    {
        private UnityEditor.Editor _cachedEditor;
        private UnityEditor.Editor _cachedEditor2;
        private bool _foldout1;
        private bool _foldout2;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(_self.CurrentBrush, ref _foldout1, ref _cachedEditor2);
            DrawSettingsEditor(_self.WorldSettings, ref _foldout2, ref _cachedEditor);
        }
    }
}