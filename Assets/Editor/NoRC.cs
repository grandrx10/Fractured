#if UNITY_EDITOR
// using UnityEditor;
// using UnityEngine;
//
// [InitializeOnLoad]
// public static class DisableSceneViewContextMenuOnly
// {
//     static DisableSceneViewContextMenuOnly()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//     }
//
//     static void OnSceneGUI(SceneView sceneView)
//     {
//         Event e = Event.current;
//         //if (e.type != EventType.Layout && e.type != EventType.Repaint) Debug.Log(e.type);
//         // This is what actually opens the context menu
//         if ((e.type == EventType.ContextClick || e.type == EventType.MouseUp) && e.button == 1)
//         {
//             e.Use(); // block menu, allow drag
//         }
//     }
// }
#endif