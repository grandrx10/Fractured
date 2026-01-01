using UnityEngine;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters;
using Cards;

[CreateAssetMenu(menuName = "Behaviors/BarrierBehavior")]
public class BarrierBehavior : Behavior, IBehaviorUseListener
{
    [Header("Barrier Settings")]
    public GameObject barrierPrefab; // Prefab to spawn
    public Vector3 localOffset = Vector3.zero; // Offset relative to player

    public bool Use(CardEnv env, Agent agent)
    {
        var player = OpenWorldEnv.Current.PlayerTransform;
        if (player == null)
        {
            Debug.LogWarning("BarrierBehavior: PlayerSingleton.Instance is null");
            return false;
        }

        if (barrierPrefab == null)
        {
            Debug.LogWarning("BarrierBehavior: barrierPrefab is not assigned");
            return false;
        }

        // Spawn the barrier and parent to the player
        GameObject barrierInstance = Instantiate(barrierPrefab, player.position + localOffset, Quaternion.identity);
        barrierInstance.transform.SetParent(player);
        return true;
    }
}
