using MapGen.Map;
using UnityEditor;
using UnityEngine;

namespace MapGen.Editor
{
    [CustomEditor(typeof(MapSettings))]
    public class MapSettingsEditor : UnityEditor.Editor
    {
        private MapSettings _mapSettings;
        private UnityEditor.Editor _objectPlacementNoiseSettings;
        private UnityEditor.Editor _groundNoiseSettings;
        private UnityEditor.Editor _randomSettings;
        private bool _objectPlacementFoldout;
        private bool _groundNoiseFoldout;
        private bool _randomSettingsFoldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(_mapSettings.ObjectPlacementNoise, _objectPlacementFoldout, ref _objectPlacementNoiseSettings);
            DrawSettingsEditor(_mapSettings.GroundPlacementNoise, _groundNoiseFoldout, ref _groundNoiseSettings);
            DrawSettingsEditor(_mapSettings.RandomSettings, _randomSettingsFoldout, ref _randomSettings);
        }

        private static void DrawSettingsEditor(Object settings, bool foldout, ref UnityEditor.Editor cachedEditor)
        {
            if(settings == null) return;
            
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (!foldout) return;
            CreateCachedEditor(settings, null, ref cachedEditor);
            cachedEditor.OnInspectorGUI();
        }

        private void OnEnable()
        {
            _mapSettings = (MapSettings) target;
        }
    }
}