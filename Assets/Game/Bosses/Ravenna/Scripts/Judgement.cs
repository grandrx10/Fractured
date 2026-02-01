using System;
using Cards;
using Game.Health;
using UnityEngine;
using Utils;

public class Judgement : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var main = PhysicsHelper.MainObj(other);
        if (main.TryGetComponent(out PlayerHealth pa) && main.TryGetComponent(out Rigidbody rb))
        {
            if (Vector3.Scale(rb.linearVelocity, new Vector3(1f, 0f, 1f)).magnitude > 1f) pa.TakeDamage(1, gameObject);
        }
    }
}
