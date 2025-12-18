using UnityEditor;
using UnityEngine;

public static class BindPoseBakeFlagUtility
{
    const string Flag = "BakeBindPoseToVertexColor";

    // Validate: only show for model assets
    [MenuItem("Assets/Bind Pose/Enable Bake Flag", true)]
    private static bool ValidateEnable() => Selection.activeObject is DefaultAsset || Selection.activeObject is GameObject;

    [MenuItem("Assets/Bind Pose/Enable Bake Flag")]
    private static void EnableFlag()
    {
        SetFlag(true);
    }

    [MenuItem("Assets/Bind Pose/Disable Bake Flag", true)]
    private static bool ValidateDisable() => Selection.activeObject is DefaultAsset || Selection.activeObject is GameObject;

    [MenuItem("Assets/Bind Pose/Disable Bake Flag")]
    private static void DisableFlag()
    {
        SetFlag(false);
    }

    private static void SetFlag(bool enable)
    {
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;

            string ud = importer.userData ?? "";

            bool hasFlag = ud.Contains(Flag);

            if (enable && !hasFlag)
                importer.userData = (ud + "\n" + Flag).Trim();
            else if (!enable && hasFlag)
                importer.userData = ud.Replace(Flag, "").Trim();

            importer.SaveAndReimport();
        }
    }
}