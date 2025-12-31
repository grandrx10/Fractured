using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    [Header("Scaling")]
    public float initialScaleXZ = 0.1f;
    public float targetScaleXZ = 1f;
    public float growDuration = 2f; // how long it takes to fully grow

    [Header("Materials")]
    public Material warningMaterial; // starting material
    public Material activeMaterial;  // material when fully grown

    private Renderer rend;

    void Start()
    {
        // Get Renderer
        rend = GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning("Laser: No Renderer found on this object.");
            return;
        }

        // Set initial material
        if (warningMaterial != null)
            rend.material = warningMaterial;

        // Set initial scale
        Vector3 scale = transform.localScale;
        scale.x = initialScaleXZ;
        scale.z = initialScaleXZ;
        transform.localScale = scale;

        // Start growing
        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(targetScaleXZ, startScale.y, targetScaleXZ);

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;

        // Swap to active material
        if (activeMaterial != null)
            rend.material = activeMaterial;
    }
}
