using Coffee.UISoftMaskInternal;
using UnityEditor;
using UnityEngine;

namespace Coffee.UISoftMask
{
    [CustomEditor(typeof(UISoftMaskProjectSettings))]
    internal class UISoftMaskProjectSettingsEditor : Editor
    {
        private ShaderVariantRegistryEditor _shaderVariantRegistryEditor;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUIUtility.labelWidth = 180;

            // Setting
            EditorGUILayout.LabelField("Setting", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SoftMaskEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StereoEnabled"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_TransformSensitivity"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_SoftMaskable"));

            // Editor
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_HideGeneratedComponents"));

            // Shader registry.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Shader", EditorStyles.boldLabel);
            if (_shaderVariantRegistryEditor == null)
            {
                var property = serializedObject.FindProperty("m_ShaderVariantRegistry");
                _shaderVariantRegistryEditor = new ShaderVariantRegistryEditor(property, "(SoftMaskable)",
                    () =>
                    {
                        UISoftMaskProjectSettings.shaderRegistry
                            .RegisterOptionalShaders(UISoftMaskProjectSettings.instance);
                    });
            }

            _shaderVariantRegistryEditor.Draw();

            // Advanced
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
            var excludeProp = serializedObject.FindProperty("m_ExcludeFromPreloadedAssetsWhenBuildPlayer");
            EditorGUILayout.PropertyField(excludeProp);

            if (excludeProp.boolValue)
            {
                var shadersProp = serializedObject.FindProperty("m_ShaderVariantRegistry")
                    .FindPropertyRelative("m_RegisteredShaders");
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.indentLevel++;
                for (var i = 0; i < shadersProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(shadersProp.GetArrayElementAtIndex(i), GUIContent.none);
                }
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();

                if (shadersProp.arraySize == 0)
                {
                    EditorGUILayout.HelpBox(
                        "No shaders registered. Re-import or modify the settings asset to trigger sync.",
                        MessageType.Warning);
                }
            }

            serializedObject.ApplyModifiedProperties();

            // Upgrade All Assets For V3.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Upgrade", EditorStyles.boldLabel);
            if (GUILayout.Button("Upgrade All Assets For V3"))
            {
                EditorApplication.delayCall += () => new UISoftMaskModifierRunner().RunIfUserWantsTo();
            }

            GUILayout.FlexibleSpace();
        }
    }
}
