using UnityEngine;
using Utils;

namespace Cards.Visual
{
    public class UITiltTowardMouse : MonoBehaviour
    {
        public float maxTilt = 10f;       // degrees
        public float smooth = 8f;         // lerp speed

        RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        void Update()
        {
            Vector2 mouse = Input.mousePosition;
            Camera cam = UIHelper.UICamera;
            Vector2 rtWorld = cam.WorldToScreenPoint(_rt.position);
            Vector2 dir = (mouse - rtWorld);

            // Normalize to screen range approx
            dir /= Screen.height;

            float tiltX = dir.y * maxTilt;
            float tiltY =  -dir.x * maxTilt;

            Quaternion targetRot = Quaternion.Euler(tiltX, tiltY, 0);
            _rt.localRotation = Quaternion.Lerp(_rt.localRotation, targetRot, Time.deltaTime * smooth);
        }
    }
}