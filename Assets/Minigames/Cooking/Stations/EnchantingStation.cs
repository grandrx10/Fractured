using UnityEngine;
using System.Collections;
using Characters;
using Characters.Interactables;

public class EnchantingStation : Station
{

    private Coroutine currentEnchantRoutine;
    private GameObject currentObjectInStation;

    public override void Interact(GameObject player)
    {
        // Get the cook
        Cook cook = PlayerSingleton.Instance.GetComponent<Cook>();
        if (cook == null) return;

        Cookable heldObject = cook.heldObject;
        if (heldObject == null) return;

        Enchantable enchantable = heldObject.GetComponent<Enchantable>();
        if (enchantable == null || enchantable.transformedPrefab == null)
        {
            Debug.LogWarning("Held object is not enchantable or missing transformed prefab.");
            Explode();
            return;
        }

        // Move the object to the station
        heldObject.transform.position = stationPoint.position;
        heldObject.transform.rotation = stationPoint.rotation;
        heldObject.transform.parent = stationPoint;

        // Make it kinematic while in station
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Remove from cook's hand
        cook.heldObject = null;

        // Cancel previous enchant if any
        if (currentEnchantRoutine != null)
            StopCoroutine(currentEnchantRoutine);

        currentObjectInStation = heldObject.gameObject;
        currentEnchantRoutine = StartCoroutine(HandleEnchant(enchantable));
    }

    private IEnumerator HandleEnchant(Enchantable enchantable)
    {
        float elapsed = 0f;

        // Wait for the enchant duration
        while (elapsed < enchantable.enchantDuration)
        {
            // If the object was picked up, cancel
            if (currentObjectInStation == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Transform object
        if (currentObjectInStation != null)
        {
            Vector3 pos = currentObjectInStation.transform.position;
            Quaternion rot = currentObjectInStation.transform.rotation;

            Destroy(currentObjectInStation);

            GameObject newObj = Instantiate(enchantable.transformedPrefab, pos, rot, stationPoint);
            
            // Add Enchantable component if not present
            Enchantable newEnchantable = newObj.GetComponent<Enchantable>();
            if (newEnchantable == null)
                newEnchantable = newObj.AddComponent<Enchantable>();

            newEnchantable.isEnchanted = true;

            currentObjectInStation = newObj;

            // Make it pick-upable
            Rigidbody rb = newObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
            }

            // Wait expireTime for pick-up
            elapsed = 0f;
            while (elapsed < enchantable.expireTime)
            {
                // If picked up (no longer parented to station)
                if (currentObjectInStation == null || currentObjectInStation.transform.parent != stationPoint)
                {
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // If still in station after expireTime, explode
            if (currentObjectInStation != null)
            {
                Destroy(currentObjectInStation);
                Explode();
            }
        }
    }
}