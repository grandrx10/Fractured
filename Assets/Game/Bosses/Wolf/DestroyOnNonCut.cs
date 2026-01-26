using UnityEngine;
using Cards.Card_Assets.RPS.Behaviors;
public class DestroyOnNonCut : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
        {
            // Ignore collisions with cuttable objects
            var cuttable = other.GetComponent<ICuttable>();
            if (cuttable != null)
                return;

            // Otherwise, destroy self
            Destroy(gameObject);
        }
}
