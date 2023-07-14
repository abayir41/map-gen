using LabGen.Labyrinth;
using MapGen.Map.Brushes;
using MapGen.Map.Brushes.Labyrinth;
using UnityEditor;
using UnityEngine;

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
            DrawSettingsEditor(_self.LabyrinthBrushSettings, ref _foldout, ref _cachedEditor);
        }
    }
}