#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Utils;

[CustomPropertyDrawer(typeof(FloatBindingDefinition))]
public class FloatBindingDefinitionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var objProp = property.FindPropertyRelative("targetObject");
        var compProp = property.FindPropertyRelative("targetComponent");
        var memberProp = property.FindPropertyRelative("memberName");
        var setterProp = property.FindPropertyRelative("setterEnabled");

        var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        // 1. GameObject field
        EditorGUI.PropertyField(line, objProp);
        line.y += EditorGUIUtility.singleLineHeight + 2;

        GameObject go = objProp.objectReferenceValue as GameObject;
        if (go != null)
        {
            // 2. Component dropdown
            var components = go.GetComponents<Component>();
            var componentNames = components.Select(c => c.GetType().Name).ToArray();

            int index = Mathf.Max(0, System.Array.IndexOf(components, compProp.objectReferenceValue));
            int newIndex = EditorGUI.Popup(line, "Component", index, componentNames);

            if (newIndex != index)
                compProp.objectReferenceValue = components[newIndex];

            line.y += EditorGUIUtility.singleLineHeight + 2;

            // 3. Member dropdown (float fields + properties)
            if (compProp.objectReferenceValue != null)
            {
                var comp = compProp.objectReferenceValue;
                var type = comp.GetType();

                var floatMembers =
                    type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(f => f.FieldType == typeof(float))
                        .Select(f => f.Name)
                        .ToList();

                floatMembers.AddRange(
                    type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.PropertyType == typeof(float))
                        .Select(p => p.Name)
                );

                int mIndex = Mathf.Max(0, floatMembers.IndexOf(memberProp.stringValue));
                if (mIndex == -1 || mIndex > floatMembers.Count) mIndex = 0;
                int mNewIndex = EditorGUI.Popup(line, "Member", mIndex, floatMembers.ToArray());

                if ((mNewIndex != mIndex || newIndex != index) && floatMembers.Count > 0)
                    memberProp.stringValue = floatMembers[mNewIndex];

                line.y += EditorGUIUtility.singleLineHeight + 2;

                // 4. Allow writing toggle
                EditorGUI.PropertyField(line, setterProp);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var objProp = property.FindPropertyRelative("targetObject");
        var compProp = property.FindPropertyRelative("targetComponent");

        int lines = 1; // GameObject

        if (objProp.objectReferenceValue != null)
        {
            lines++; // Component
            if (compProp.objectReferenceValue != null)
                lines += 2; // Member + Setter
        }

        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
#endif
