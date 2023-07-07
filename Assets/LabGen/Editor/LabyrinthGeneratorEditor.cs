using LabGen.Labyrinth;
using UnityEditor;
using UnityEngine;

namespace LabGen.Editor
{
    [CustomEditor(typeof(LabyrinthGenerator))]
    public class LabyrinthGeneratorEditor : UnityEditor.Editor
    {
        private LabyrinthGenerator _mapGenerator;
        private UnityEditor.Editor _labEditor;
        private bool _foldout;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate Labyrinth"))
            {
                _mapGenerator.GenerateLabyrinth();
            }
            
            DrawSettingsEditor(_mapGenerator.LabyrinthSettings, ref _foldout, ref _labEditor);
        }

        private void DrawSettingsEditor(Object settings, ref bool foldout, ref UnityEditor.Editor cachedEditor)
        {
            if(settings == null) return;
            
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (!foldout) return;
            
            CreateCachedEditor(settings, null, ref cachedEditor);
            cachedEditor.OnInspectorGUI();
        }

        private void OnEnable()
        {
            _mapGenerator = (LabyrinthGenerator) target;
        }
    }
}