using MapGen.Map.Brushes.Labyrinth;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(LabyrinthBrush))]
    public class LabyrinthBrushEditor : SettingDrawer<LabyrinthBrush>
    {
        private UnityEditor.Editor _cachedEditor;
        private bool _foldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(Self.LabyrinthBrushSettings, ref _foldout, ref _cachedEditor);
        }
    }
}