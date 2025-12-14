using UnityEngine;
using Characters;
using Characters.Interactables;

public class Station : Interactable
{

    [Tooltip("Point where the prefab should appear")]
    public Transform stationPoint;

    private void Update()
    {
        // Make sure player exists
        if (PlayerSingleton.Instance == null)
        {
            canInteract = false;
            return;
        }

        // Get the Cook component from the singleton
        Cook cook = PlayerSingleton.Instance.GetComponent<Cook>();
        if (cook == null || cook.heldObject == null)
        {
            canInteract = false;
            return;
        }

        // Check if the held object meets criteria
        canInteract = CheckCriteria(cook.heldObject);
    }

    public override void Interact(GameObject player)
    {
    }

    protected virtual bool CheckCriteria(Cookable heldObject)
    {
        return true;
    }

    /// <summary>
    /// Spawn the assigned prefab at the stationPoint position and rotation.
    /// </summary>
    public virtual void Explode()
    {
        if (ExplodeManager.Instance != null)
        {
            ExplodeManager.Instance.Explode(stationPoint);
        }
    }
}
