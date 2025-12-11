using UnityEngine;

namespace Cards.PhysicalProperties
{
    [RequireComponent(typeof(Rigidbody))]
    public class CurveTowards : MonoBehaviour
    {
        public float duration = 1f;
        public AnimationCurve tangentInfluence;

        private Rigidbody _rb;
        private PhysicalObject.PhysicalInitState _state;

        private float _length;
        private Vector3 _p0, _p1, _center, _t0, _t1;
        private Vector3 _normal;
        [SerializeField] private float startGap, endGap, startTanStr;
        private float _elapsedTime, _speed;
        private bool _isMoving;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.isKinematic = true;

            var pco = GetComponent<PhysicalObject>();
            if (!pco) Debug.LogError("expected PhysicalCardObject");

            pco.OnInit += state =>
            {
                _state = state;

                var sTan = 1 - (Vector3.Dot(state.StartDirection, state.TargetDirection) / 2 + 0.5f);
                
                _p0 = state.StartPosition + state.StartDirection.normalized * startGap;
                _t0 = state.StartDirection.normalized * (startTanStr * sTan);       // Start tangent
                _t1 = state.TargetDirection.normalized;      // Exit tangent
                _center = state.CenterPosition;
                _p1 = _center + state.TargetDirection * endGap;        // Exit past center
                _speed = state.Speed;
                _elapsedTime = 0f;
                _isMoving = true;
                _rb.isKinematic = true;
                _normal = Quaternion.AngleAxis(Random.Range(-15, 15), _t1) * Vector3.up;
                _length = ApproximateCurveLength();
            };
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;

            _elapsedTime += Time.fixedDeltaTime / Mathf.Sqrt(_length);
            float t = Mathf.Clamp01(_elapsedTime / duration);

            // Compute the smooth curve with tangents
            Vector3 point = TangentArc(_p0, _p1, _center, _t0, _t1, t);

            transform.position = point + (_state.Target.position - _state.CenterPosition);
            var tang = TangentAtT(t);
            var sidew = Vector3.Cross(_t1, Vector3.up);
            var upTarg = Vector3.Slerp(Vector3.up, sidew, t);
            if (tang != Vector3.zero) transform.rotation = Quaternion.LookRotation(TangentAtT(t), upTarg);
            if (t >= 1f)
            {
                var pco = GetComponent<PhysicalObject>();
                pco.Move(new PhysicalObject.PhysicalActiveState()
                {
                    StartPosition = _p1,
                    StartDirection = _t1,
                    Speed = _speed,
                });
                Destroy(this);
            }
        }
    
        public float ApproximateCurveLength(int steps = 50)
        {
            if (!_isMoving) return 0f;

            float length = 0f;
            Vector3 prev = TangentArc(_p0, _p1, _center, _t0, _t1, 0f);

            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector3 curr = TangentArc(_p0, _p1, _center, _t0, _t1, t);
                length += Vector3.Distance(prev, curr);
                prev = curr;
            }

            return length;
        }
    
        Vector3 TangentAtT(float t)
        {
            // Small delta for finite difference approximation
            float dt = 0.001f;
            float t1 = Mathf.Clamp01(t + dt);

            Vector3 pos0 = TangentArc(_p0, _p1, _center, _t0, _t1, t);
            Vector3 pos1 = TangentArc(_p0, _p1, _center, _t0, _t1, t1);

            Vector3 tangent = (pos1 - pos0).normalized;
            return tangent;
        }

        /// <summary>
        /// Blends a circular arc with tangents at start and end for smooth transitions
        /// </summary>
        private Vector3 TangentArc(Vector3 start, Vector3 end, Vector3 center, Vector3 tangentStart, Vector3 tangentEnd, float t)
        {
            // Circular arc component
            Vector3 startDir = start - center;
            Vector3 endDir = end - center;

            Vector3 normal = _normal; // fallback if collinear

            startDir = Vector3.ProjectOnPlane(startDir, normal);
            endDir = Vector3.ProjectOnPlane(endDir, normal);

            float angle = Vector3.SignedAngle(startDir, endDir, normal);
            Vector3 arcDir = Quaternion.AngleAxis(angle * t, normal) * startDir;
            Vector3 arcPos = center + arcDir;

            // Tangent component (Hermite blending)
            Vector3 tangentPos = (1 - t) * (start + tangentStart * t) + t * (end - tangentEnd * (1 - t));

            // Blend arc and tangents
            return Vector3.Lerp(arcPos, tangentPos, tangentInfluence.Evaluate(t));
        }

        private void OnDrawGizmos()
        {
            if (!_isMoving) return;

            Vector3 prev = _p0;
            int steps = 20;
            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector3 point = TangentArc(_p0, _p1, _center, _t0, _t1, t);
                Gizmos.DrawLine(prev, point);
                prev = point;
            }
        }
    }
}
