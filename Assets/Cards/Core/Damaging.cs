using UnityEngine;
using Cards.Core;
using Game.Health;

public class Damaging : MonoBehaviour
{
    [Header("Card Reference")]
    public Card card;

    [Header("Damage Settings")]
    public bool dealDamageOnce = true;

    [Header("Damage Options")]
    public bool damageOnTrigger = true;
    public bool damageOnCollide = true;
    public bool destroyAfterDamage = false;

    [Header("Destroy on Touch Settings")]
    [Tooltip("Layers that will destroy this object immediately on contact")]
    public LayerMask destroyOnTouchLayers;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool hasDealtDamage = false;

    private void Start()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[Damaging] Starting on {gameObject.name}");
            Debug.Log($"[Damaging] Card assigned: {(card != null ? card.name : "NULL")}");
            if (card != null)
                Debug.Log($"[Damaging] Card strength: {card.stats.strength}");
        }

        CheckInitialOverlap();
    }

    private void CheckInitialOverlap()
    {
        if (!damageOnTrigger) return;

        Collider myCollider = GetComponent<Collider>();
        if (!myCollider)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[Damaging] No collider found on {gameObject.name}");
            return;
        }

        Collider[] hits = Physics.OverlapBox(
            myCollider.bounds.center,
            myCollider.bounds.extents,
            transform.rotation
        );

        if (showDebugLogs)
            Debug.Log($"[Damaging] Initial overlap found {hits.Length} colliders");

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (showDebugLogs)
                Debug.Log($"[Damaging] Checking overlap with: {hit.gameObject.name}");
            
            ApplyDamage(hit.gameObject, true); // true = from initial overlap
            CheckDestroyOnTouch(hit.gameObject);
        }
    }

    private void ApplyDamage(GameObject target, bool ignoreFlags = false)
    {
        if (showDebugLogs)
            Debug.Log($"[Damaging] ApplyDamage called on: {target.name}");

        if (!ignoreFlags && dealDamageOnce && hasDealtDamage)
        {
            if (showDebugLogs)
                Debug.Log($"[Damaging] Already dealt damage once, skipping");
            return;
        }

        if (card == null)
        {
            if (showDebugLogs)
                Debug.LogWarning($"[Damaging] Card is NULL on {gameObject.name}");
            return;
        }

        var health = target.GetComponentInParent<Health>() ??
                     target.GetComponent<Health>() ??
                     target.GetComponentInChildren<Health>();

        if (health != null)
        {
            int damage = card.stats.strength * 10;
            if (showDebugLogs)
                Debug.Log($"[Damaging] Dealing {damage} damage to {target.name}");

            health.TakeDamage(damage);
            hasDealtDamage = true;

            if (destroyAfterDamage)
            {   
                if (showDebugLogs)
                    Debug.Log("Destroyed after damage");

                Destroy(gameObject); 
                return;  
            }
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"[Damaging] No Health component found on {target.name} or its parents/children");
        }
        CheckDestroyOnTouch(target);
    }

    private void CheckDestroyOnTouch(GameObject target)
    {
        if (((1 << target.layer) & destroyOnTouchLayers) != 0)
        {
            if (showDebugLogs)
                Debug.Log($"[Damaging] Destroyed on touch with {target.name} (layer {target.layer})");

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damageOnCollide)
        {
            if (showDebugLogs)
                Debug.Log($"[Damaging] OnCollisionEnter with: {collision.gameObject.name}");
            ApplyDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageOnTrigger)
        {
            if (showDebugLogs)
                Debug.Log($"[Damaging] OnTriggerEnter with: {other.gameObject.name}");
            ApplyDamage(other.gameObject);
        }
    }
}
