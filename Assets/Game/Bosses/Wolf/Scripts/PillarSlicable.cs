using UnityEngine;
using Cards.Card_Assets.RPS.Behaviors;
using System.Collections;

public class PillarSliceable : Slice, ICuttable
{
    [Header("Shrink Settings")]
    public float shrinkDuration = 1f; // seconds to scale down
    public float delayBeforeShrink = 3f; // wait before starting shrink

    public void Cut(Vector3 cutNormal, Vector3 cutPosition)
    {
        ComputeSlice(cutNormal, cutPosition);

        // Make debris walkable (static)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
        }

        // Find the root object and add a temporary component to handle the shrinking
        GameObject rootObj = transform.root.gameObject;
        var rbs = rootObj.GetComponentsInChildren<Rigidbody>(false);
        
        foreach (Rigidbody r in rbs) {
            
            var s = r.gameObject.AddComponent<ShrinkAndDestroy>();
            s.shrinkDuration = shrinkDuration;
            s.delayBeforeShrink = delayBeforeShrink;
            s.StartShrinking();
        }
    }
}

// Separate component to handle shrinking - attach this to the root
public class ShrinkAndDestroy : MonoBehaviour
{
    public float shrinkDuration = 1f;
    public float delayBeforeShrink = 3f;
    private bool isStarted = false;

    public void StartShrinking()
    {
        if (!isStarted)
        {
            isStarted = true;
            StartCoroutine(DelayedShrinkAndDestroy());
        }
    }

    private IEnumerator DelayedShrinkAndDestroy()
    {
        // Wait before starting shrink
        yield return new WaitForSeconds(delayBeforeShrink);

        Vector3 initialScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / shrinkDuration);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}