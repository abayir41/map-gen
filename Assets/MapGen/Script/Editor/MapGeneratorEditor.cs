﻿using MapGen.Map;
using UnityEditor;
using UnityEngine;

namespace MapGen.Editor
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : UnityEditor.Editor {
        
        private MapGenerator _mapGenerator;
        private UnityEditor.Editor _mapEditor;
        private UnityEditor.Editor _noiseSettings;
        private UnityEditor.Editor _randomSettings;
        private bool _foldout;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Generate Map"))
            {
                _mapGenerator.GenerateMap();
            }
            
            DrawSettingsEditor(_mapGenerator.MapSettings, _foldout, ref _mapEditor);
        }

        private void DrawSettingsEditor(Object settings, bool foldout, ref UnityEditor.Editor cachedEditor)
        {
            if(settings == null) return;
            
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (!foldout) return;
            
            CreateCachedEditor(settings, null, ref cachedEditor);
            cachedEditor.OnInspectorGUI();
        }

        private void OnEnable()
        {
            _mapGenerator = (MapGenerator) target;
        }
    }
}