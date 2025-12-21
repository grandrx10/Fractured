using UnityEngine;

namespace Utils
{
    public static class UIHelper
    {
        
        public static Camera UICamera => GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
        /// <summary>
        /// Returns the top-level Canvas that contains this UI element
        /// </summary>
        public static Canvas GetRootCanvas(Transform element)
        {
            Canvas lastCanvas = null;
            Transform t = element;

            while (t != null)
            {
                Canvas c = t.GetComponent<Canvas>();
                if (c != null)
                    lastCanvas = c;

                t = t.parent;
            }

            return lastCanvas;
        }
        
        public static void FlattenChildrenZ(Transform parent)
        {
            foreach (Transform child in parent)
            {
                Vector3 pos = child.localPosition;
                pos.z = 0;
                child.localPosition = pos;

                // Recursive call for grandchildren
                FlattenChildrenZ(child);
            }
        }
    }
}