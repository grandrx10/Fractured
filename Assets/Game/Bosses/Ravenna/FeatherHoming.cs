using UnityEngine;

public class FeatherHoming : MonoBehaviour
{
    private Vector3 _target;
    private float _speed;
    private float _turnSpeed;
    private float _killDistance;

    private Rigidbody _rb;

    public void Initialize(
        Vector3 target,
        float speed,
        float turnSpeed,
        float killDistance
    )
    {
        _target = target;
        _speed = speed;
        _turnSpeed = turnSpeed;
        _killDistance = killDistance;

        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _rb.useGravity = false;
    }

    void FixedUpdate()
    {
        if (_rb == null)
            return;

        // 🔹 Check distance FIRST
        if (Vector3.SqrMagnitude(_target - transform.position) <= _killDistance * _killDistance)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 desiredDir = (_target - transform.position).normalized;

        Vector3 newDir = Vector3.RotateTowards(
            transform.forward,
            desiredDir,
            _turnSpeed * Time.fixedDeltaTime,
            0f
        );

        _rb.MoveRotation(Quaternion.LookRotation(newDir, Vector3.up));
        _rb.linearVelocity = newDir * _speed;
    }
}
