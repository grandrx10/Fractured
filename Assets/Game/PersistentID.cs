using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class PersistentID : MonoBehaviour
{
    [SerializeField]
    private string id;

    public string ID => id;

    void Awake()
    {
        EnsureID();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        EnsureID();
    }
#endif

    void EnsureID()
    {
#if UNITY_EDITOR
        if (PrefabUtility.IsPartOfPrefabAsset(this))
            return;
#endif

        if (!string.IsNullOrEmpty(id))
            return;
        if (!HasDuplicate(id))
            return;
#if UNITY_EDITOR
        Undo.RecordObject(this, "Generate Persistent ID");
#endif

        id = Guid.NewGuid().ToString();

#if UNITY_EDITOR
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }

    bool HasDuplicate(string candidate)
    {
        // Find all PersistentID in loaded scenes
        var all = FindObjectsOfType<PersistentID>(true);

        foreach (var other in all)
        {
            if (other == this) continue;
            if (other.id == candidate)
                return true;
        }
        return false;
    }
}