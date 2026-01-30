using UnityEngine;
using System;

public class RiseTrigger : MonoBehaviour
{
    public LayerMask playerLayer;
    public bool triggerOnce = true;

    public event Action OnTriggered;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        hasTriggered = true;
        OnTriggered?.Invoke();
    }
}
