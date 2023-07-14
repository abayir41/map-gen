using MapGen.Map.Brushes.NormalMap;
using UnityEditor;

namespace MapGen.Editor
{
    [CustomEditor(typeof(MapBrushSettings))]
    public class MapBrushSettingsEditor : SettingDrawer<MapBrushSettings>
    {
        private UnityEditor.Editor _objectPlacementNoiseSettings;
        private UnityEditor.Editor _groundNoiseSettings;
        private UnityEditor.Editor _randomSettings;
        private bool _objectPlacementFoldout;
        private bool _groundNoiseFoldout;
        private bool _randomSettingsFoldout;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawSettingsEditor(Self.ObjectPlacementNoise, ref _objectPlacementFoldout, ref _objectPlacementNoiseSettings);
            DrawSettingsEditor(Self.GroundPlacementNoise, ref _groundNoiseFoldout, ref _groundNoiseSettings);
            DrawSettingsEditor(Self.RandomSettings, ref _randomSettingsFoldout, ref _randomSettings);
        }
    }
}