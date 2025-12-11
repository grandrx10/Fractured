using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PrefabComponentAttribute))]
public class PrefabComponentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Calculate rects
        float buttonWidth = 50f;
        Rect fieldRect = new Rect(position.x, position.y, position.width - buttonWidth - 4, position.height);
        Rect buttonRect = new Rect(fieldRect.xMax + 4, position.y, buttonWidth, position.height);

        // Draw the normal field (shrunk so the circle stays away)
        EditorGUI.PropertyField(fieldRect, property, label);

        // Draw your button in its own space
        if (GUI.Button(buttonRect, "Pick"))
        {
            PrefabComponentPicker.Open(property);
        }

        EditorGUI.EndProperty();
    }
}