using System;
using UnityEngine;

public class DiscreteBoxDetector : MonoBehaviour
{
    [Header("Detector State")]
    [Tooltip("True when a DiscretePushBox is on this detector")]
    public bool isActive => currentBox && !currentBox.isMoving;

    public string requiredId;
    private DiscretePushBox currentBox;

    private void OnTriggerEnter(Collider other)
    {
        DiscretePushBox box = other.GetComponentInParent<DiscretePushBox>();

        if (box == null || (box.id != requiredId && requiredId != ""))
            return;

        currentBox = box;

    }

    private void OnTriggerExit(Collider other)
    {
        DiscretePushBox box = other.GetComponentInParent<DiscretePushBox>();
        if (box == null)
            return;

        if (box == currentBox)
        {
            currentBox = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
#endif
}
