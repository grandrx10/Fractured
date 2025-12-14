using UnityEngine;

namespace Utils
{
    public static class UIHelper
    {
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
            float parentZ = parent.position.z;

            foreach (Transform child in parent)
            {
                Vector3 pos = child.position;
                pos.z = parentZ;
                child.position = pos;

                // Recursive call for grandchildren
                FlattenChildrenZ(child);
            }
        }
    }
}