using System;
using UnityEngine;
using Utils;

public class WaterJump : MonoBehaviour
{
    public float waitTime;
    private float _time;
    public float force;
    public GameObject explosion;
    private void OnTriggerStay(Collider other)
    {
        var m = PhysicsHelper.MainObj(other);
        if (m.CompareTag("Player"))
        {
            _time += Time.deltaTime;
            if (_time > waitTime) Fire(m);
        }
    }

    private void Fire(GameObject go)
    {
        _time = 0;
        go.GetComponent<Rigidbody>().AddForce(Vector3.up * force, ForceMode.Impulse);
        Instantiate(explosion, go.transform.position, Quaternion.identity);
    }

    private void OnTriggerExit(Collider other)
    {
        var m = PhysicsHelper.MainObj(other);
        if (m.CompareTag("Player"))
        {
            _time = 0;
        }
    }
}
