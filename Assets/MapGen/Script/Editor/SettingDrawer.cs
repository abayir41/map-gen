using UnityEditor;
using UnityEngine;

namespace MapGen.Editor
{
    public class SettingDrawer<T> : UnityEditor.Editor where T : Object
    {
        protected T Self { get; private set; }

        protected static void DrawSettingsEditor(Object settings, ref bool foldout, ref UnityEditor.Editor cachedEditor)
        {
            if(settings == null) return;
            
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            if (!foldout) return;
            CreateCachedEditor(settings, null, ref cachedEditor);
            cachedEditor.OnInspectorGUI();
        }

        private void OnEnable()
        {
            Self = (T) target;
        }
    }
}