using UnityEngine;

public abstract class BossAttack : ScriptableObject
{
    public float attackDuration = 2f; // Duration of attack
    public float delayAfter = 1f;     // Time to wait after attack
    public bool triggerOnce = false;
    public bool hasTriggered = false;

    public bool isActive { get; private set; }

    public virtual void StartAttack(GameObject boss)
    {
        if (triggerOnce && hasTriggered) return;
        isActive = true;
        hasTriggered = true;
    }

    // Make Tick virtual so subclasses can override
    public virtual void Tick(GameObject boss) { }

    public virtual void EndAttack(GameObject boss)
    {
        isActive = false;
    }
}
