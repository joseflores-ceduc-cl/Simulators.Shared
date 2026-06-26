using UnityEditor;
using UnityEngine;

namespace Ceduc.SWMiningTruck
{
    [CustomEditor(typeof(ButtonBaseUI))]
    [CanEditMultipleObjects]
    public class ButtonBaseUIEditor : UnityEditor.UI.ButtonEditor
    {
        private SerializedProperty label;
        private SerializedProperty disabledTextColor;
        private SerializedProperty normalTextColor;

        protected override void OnEnable()
        {
            base.OnEnable();
            label = serializedObject.FindProperty("label");
            disabledTextColor = serializedObject.FindProperty("disabledTextColor");
            normalTextColor = serializedObject.FindProperty("normalTextColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI(); // Inspector base (Button)

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button Base UI", EditorStyles.boldLabel);

            if (label != null)
                EditorGUILayout.PropertyField(label);
            if (disabledTextColor != null)
                EditorGUILayout.PropertyField(disabledTextColor);
            if (normalTextColor != null)
                EditorGUILayout.PropertyField(normalTextColor);

            serializedObject.ApplyModifiedProperties();
        }
 
    }

}