using UnityEngine;
using Cards.Core;
using Game.Health;

public class Damaging : MonoBehaviour
{
    [Header("Card Reference")]
    public Card card;

    [Header("Damage Settings")]
    public bool dealDamageOnce = true;

    private bool hasDealtDamage = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (dealDamageOnce && hasDealtDamage)
            return;

        var health = collision.gameObject.GetComponent<Health>();
        if (health != null && card != null)
        {
            int damage = card.stats.strength * 10; 
            health.TakeDamage(damage);
            hasDealtDamage = true;
        }
    }
}
