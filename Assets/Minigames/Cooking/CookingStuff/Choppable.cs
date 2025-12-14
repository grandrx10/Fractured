using UnityEngine;
using Characters.Interactables;

public class Choppable : MonoBehaviour
{
    // Called when the object is chopped
    public virtual void Chop()
    {
        Debug.Log($"{gameObject.name} has been chopped!");

        // Example: destroy the object after chopping
        // Destroy(gameObject);

        // You can also trigger particle effects, animations, etc.
    }
}
