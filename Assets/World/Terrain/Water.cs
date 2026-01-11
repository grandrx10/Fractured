using System;
using UnityEngine;
using Utils;

public class Water : MonoBehaviour
{
    public float force;
    public float drag;
    public float heightOffset;
    public AnimationCurve forceCurve;

    private void OnTriggerStay(Collider other)
    {
        var m = PhysicsHelper.MainObj(other);
        if (m.CompareTag("Player"))
        {
            var d = heightOffset + transform.position.y - m.transform.position.y;
            if (d < 0) return;
            var rb = m.GetComponent<Rigidbody>();
            rb.AddForce(forceCurve.Evaluate(d) * force * Vector3.up, ForceMode.Force);
            rb.linearVelocity -= rb.linearVelocity * Time.fixedDeltaTime * drag;
        }
    }
}
