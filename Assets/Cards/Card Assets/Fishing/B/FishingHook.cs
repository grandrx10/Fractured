using UnityEngine;
using Cards.Card_Assets.Fishing.B;
using Characters.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class FishingHook : MonoBehaviour
{
    public FishingBehaviour behaviour;
    private Rigidbody rb;
    private bool stopped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (stopped) return;
        stopped = true;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Notify behaviour if needed
        if (behaviour != null)
        {
            Debug.Log("Hook stopped, behaviour can respond.");
        }

        // Activate FishingPool on parent
        FishingPool pool = collision.collider.GetComponentInParent<FishingPool>();
        if (pool != null)
        {
            pool.Activate();
            behaviour.state = "hooked";

            // ✅ FIXED: Use += instead of = to ADD a subscription, not replace it
            if (pool.fishingMinigame != null)
            {
                pool.fishingMinigame.OnMinigameComplete += (success) =>
                {
                    if (success)
                        behaviour?.OnFishingSuccess();
                    else
                        behaviour?.OnFishingFailure();
                };
            }
        }
    }
}