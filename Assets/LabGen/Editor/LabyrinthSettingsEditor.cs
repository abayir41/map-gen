using LabGen.Labyrinth;
using MapGen.Map;
using UnityEditor;
using UnityEngine;

namespace LabGen.Editor
{
    [CustomEditor(typeof(LabyrinthSettings))]
    public class LabyrinthSettingsEditor : UnityEditor.Editor
    {
        private LabyrinthSettings _labyrinthSettings;
        private UnityEditor.Editor _randomSettings;
        private bool _randomSettingsFoldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(_labyrinthSettings.RandomSettings, ref _randomSettingsFoldout, ref _randomSettings);
        }

        private static void DrawSettingsEditor(Object settings, ref bool foldout, ref UnityEditor.Editor cachedEditor)
        {
            if(settings == null) return;
            
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (!foldout) return;
            CreateCachedEditor(settings, null, ref cachedEditor);
            cachedEditor.OnInspectorGUI();
        }

        private void OnEnable()
        {
            _labyrinthSettings = (LabyrinthSettings) target;
        }
    }
}