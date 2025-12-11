using UnityEditor;
using UnityEngine;

public class PrefabComponentPicker : EditorWindow
{
    private static SerializedProperty targetProp;
    private static System.Type fieldType;
    private Vector2 scroll;

    public static void Open(SerializedProperty prop)
    {
        targetProp = prop;
        fieldType = GetFieldType(prop);

        var win = GetWindow<PrefabComponentPicker>("Pick Component");
        win.minSize = new Vector2(350, 300);
        win.ShowUtility();
    }

    private static System.Type GetFieldType(SerializedProperty prop)
    {
        var target = prop.serializedObject.targetObject;
        var t = target.GetType();
        var field = t.GetField(
            prop.propertyPath,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        return field.FieldType;
    }

    private void OnGUI()
    {
        if (targetProp == null || fieldType == null)
        {
            Close();
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll);

        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        float size = 80f;
        int perRow = Mathf.FloorToInt((position.width - 10) / (size + 10));
        if (perRow < 1) perRow = 1;

        int col = 0;

        GUILayout.BeginHorizontal();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (!prefab) continue;

            // Does prefab contain this component?
            var comp = prefab.GetComponent(fieldType);
            if (!comp) continue;

            // Get thumbnail
            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (!preview)
                preview = AssetPreview.GetMiniThumbnail(prefab);

            // Draw a button with the preview
            GUILayout.BeginVertical(GUILayout.Width(size));

            if (GUILayout.Button(preview, GUILayout.Width(size), GUILayout.Height(size)))
            {
                targetProp.objectReferenceValue = comp;
                targetProp.serializedObject.ApplyModifiedProperties();
                Close();
            }

            // Label under preview
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false
            };
            GUILayout.Label(prefab.name, labelStyle, GUILayout.Width(size));

            GUILayout.EndVertical();

            col++;
            if (col >= perRow)
            {
                col = 0;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
            }
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
}
