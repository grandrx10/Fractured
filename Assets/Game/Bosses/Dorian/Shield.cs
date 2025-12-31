using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Shield State")]
    public bool isActive = true;
    public float reactivateDelay = 40f;

    [Header("References")]
    public GameObject visualRoot;   // VFX / ball object

    private Collider shieldCollider;
    private int playerProjectileLayer;
    private Coroutine reactivateRoutine;

    private void Awake()
    {
        shieldCollider = GetComponent<Collider>();
        playerProjectileLayer = LayerMask.NameToLayer("PlayerProjectile");

        SetShieldActive(isActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        if (other.gameObject.layer != playerProjectileLayer)
            return;

        DeactivateShield();
    }

    private void DeactivateShield()
    {
        isActive = false;
        SetShieldActive(false);

        if (reactivateRoutine != null)
            StopCoroutine(reactivateRoutine);

        reactivateRoutine = StartCoroutine(ReactivateAfterDelay());
    }

    private IEnumerator ReactivateAfterDelay()
    {
        yield return new WaitForSeconds(reactivateDelay);
        ActivateShield();
    }

    private void ActivateShield()
    {
        isActive = true;
        SetShieldActive(true);
    }

    private void SetShieldActive(bool active)
    {
        if (shieldCollider != null)
            shieldCollider.enabled = active;

        if (visualRoot != null)
            visualRoot.SetActive(active);
    }
}
