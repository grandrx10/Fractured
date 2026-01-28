using UnityEngine;

public class DiscreteBoxExploder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DiscretePushBox box = other.GetComponentInParent<DiscretePushBox>();
        if (box == null)
            return;
        box.canInteract = false;
        box.GetComponent<Rigidbody>().isKinematic = false;
        box.StopAllCoroutines();
    }
}
