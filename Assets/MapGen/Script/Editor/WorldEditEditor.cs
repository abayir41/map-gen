using MapGen.Map.MapEdit;
using MapGen.Map.MapEdit.Brushes;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(WorlEdit))]
    public class WorldEditEditor : SettingDrawer<WorlEdit>
    {
        private UnityEditor.Editor _cachedEditor;
        private bool _foldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(_self.CurrentSelectBrush, ref _foldout, ref _cachedEditor);
        }
    }
}