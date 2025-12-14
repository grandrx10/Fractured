using UnityEngine;

namespace Cards.Visual
{
    public class UITiltTowardMouse : MonoBehaviour
    {
        public float maxTilt = 10f;       // degrees
        public float smooth = 8f;         // lerp speed

        RectTransform rt;

        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        void Update()
        {
            Vector2 mouse = Input.mousePosition;
            Vector2 dir = (mouse - (Vector2)rt.position);

            // Normalize to screen range approx
            dir /= Screen.height;

            float tiltX = dir.y * maxTilt;
            float tiltY =  -dir.x * maxTilt;

            Quaternion targetRot = Quaternion.Euler(tiltX, tiltY, 0);
            rt.localRotation = Quaternion.Lerp(rt.localRotation, targetRot, Time.deltaTime * smooth);
        }
    }
}