using System;
using Game;
using UnityEngine;
using Utils;
using World.Objects;

public class Sandcastle : MonoBehaviour
{
    public GameObject brokenPrefab;
    private PersistentID _id;
    private void Start()
    {
        _id = GetComponent<PersistentID>();
        if (GlobalState.instance.HasEvent($"Sandcastle_Destroy_{_id.ID}")) Break();
    }

    private void Break()
    {
        var c = Instantiate(brokenPrefab, transform.position, Quaternion.identity);
        if (c.TryGetComponent(out Chest ch)) ch.id = _id;
        Destroy(gameObject);
        GlobalState.instance.AddEvent($"Sandcastle_Destroy_{_id.ID}");
    }
    private void OnCollisionEnter(Collision other)
    {
        var obj = PhysicsHelper.MainObj(other.collider);
        if (obj.CompareTag("Player"))
        {
            Break();
        }
    }
}
