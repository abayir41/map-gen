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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(_mapSettings.ObjectPlacementNoise, ref _mapSettings.objectPlacementFoldout, ref _objectPlacementNoiseSettings);
            DrawSettingsEditor(_mapSettings.GroundPlacementNoise, ref _mapSettings.groundNoiseFoldout, ref _groundNoiseSettings);
            DrawSettingsEditor(_mapSettings.RandomSettings, ref _mapSettings.randomSettingsFoldout, ref _randomSettings);
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
            _mapSettings = (MapSettings) target;
        }
    }
}