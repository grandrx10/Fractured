using System.Collections;
using UnityEngine;

public class Riser : MonoBehaviour
{
    [Header("Rise Settings")]
    public Transform targetTransform;
    public float riseDuration = 2f;
    
    [Header("Optional Trigger")]
    public RiseTrigger riseTrigger; // assign the trigger here (or via inspector)

    private Coroutine riseRoutine;
    private Rigidbody _rb;
    private void Start()
    {
        // Subscribe automatically
        if (riseTrigger != null)
        {
            riseTrigger.OnTriggered += Rise;
        }
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (riseTrigger != null)
        {
            riseTrigger.OnTriggered -= Rise;
        }
    }

    public void Rise()
    {
        Debug.Log("RISING!");
        if (targetTransform == null)
        {
            Debug.LogWarning("Riser: Target Transform is not assigned.");
            return;
        }

        if (riseRoutine != null)
            StopCoroutine(riseRoutine);

        riseRoutine = StartCoroutine(RiseRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 endPos = targetTransform.position;
        Quaternion endRot = targetTransform.rotation;

        float elapsed = 0f;

        while (elapsed < riseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / riseDuration);

            if (_rb)
            {
                _rb.position = Vector3.Lerp(startPos, endPos, t);
                _rb.rotation = Quaternion.Slerp(startRot, endRot, t);
            }
            else
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            }
            

            yield return null;
        }

        if (_rb)
        {
            _rb.position = endPos;
            _rb.rotation = endRot;
        }
        else
        {
            transform.SetPositionAndRotation(endPos, endRot);
        }
        transform.SetPositionAndRotation(endPos, endRot);
        riseRoutine = null;
    }
}
