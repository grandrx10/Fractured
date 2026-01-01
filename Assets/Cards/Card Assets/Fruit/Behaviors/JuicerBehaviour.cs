using UnityEngine;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters;
using Cards;

[CreateAssetMenu(menuName = "Behaviors/JuicerBehavior")]
public class JuicerBehavior : Behavior, IBehaviorUseListener
{
    public bool Use(CardEnv env, Agent agent)
    {
        var player = PlayerSingleton.Instance;
        if (player == null)
        {
            Debug.LogWarning("JuicerBehavior: PlayerSingleton.Instance is null");
            return false;
        }

        PlayerStatusHolder statusHolder = player.GetComponent<PlayerStatusHolder>();
        if (statusHolder == null)
        {
            Debug.LogWarning("JuicerBehavior: PlayerStatusHolder not found on player");
            return false;
        }

        // Set "juiced" status to true via float
        statusHolder.AddStatus("juiced", 1f);

        return true;
    }
}
