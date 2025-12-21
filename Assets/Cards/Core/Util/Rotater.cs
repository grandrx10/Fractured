namespace Cards.Core.Util
{
    using UnityEngine;

    public class Rotater : MonoBehaviour
    {
        public enum RotationMode
        {
            Absolute,      // Same as before
            Relative,      // Same as before
            AxisRelative   // New
        }

        [Header("Mode")]
        public RotationMode mode = RotationMode.Absolute;

        [Header("Random euler angle ranges (original behavior)")]
        public Vector2 xRange = new Vector2(0f, 0f);
        public Vector2 yRange = new Vector2(0f, 0f);
        public Vector2 zRange = new Vector2(0f, 0f);

        [Header("Axis-relative rotation (new feature)")]
        public Vector3 axis = Vector3.forward;        // local axis
        public Vector2 angleRange = new Vector2(0f, 360f);

        private void OnEnable() => ApplyRandomRotation();
        private void ApplyRandomRotation()
        {
            switch (mode)
            {
                case RotationMode.Absolute:
                    ApplyAbsolute();
                    break;

                case RotationMode.Relative:
                    ApplyRelative();
                    break;

                case RotationMode.AxisRelative:
                    ApplyAxisRelative();
                    break;
            }
        }

        // --- original behaviors ---

        private void ApplyAbsolute()
        {
            Vector3 e = RandomEuler();
            transform.rotation = Quaternion.Euler(e);
        }

        private void ApplyRelative()
        {
            Vector3 e = RandomEuler();
            transform.rotation *= Quaternion.Euler(e);
        }

        private Vector3 RandomEuler()
        {
            return new Vector3(
                Random.Range(xRange.x, xRange.y),
                Random.Range(yRange.x, yRange.y),
                Random.Range(zRange.x, zRange.y)
            );
        }

        // --- new behavior ---

        private void ApplyAxisRelative()
        {
            float angle = Random.Range(angleRange.x, angleRange.y);

            // axis specified in *local* coordinates → convert to world
            Vector3 worldAxis = transform.TransformDirection(axis.normalized);

            // rotate around that axis
            transform.rotation = Quaternion.AngleAxis(angle, worldAxis) * transform.rotation;
        }
    }
}