using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;

namespace Cards.Card_Assets.Combat.Behaviours
{
    [CreateAssetMenu(menuName = "Behaviors/DashBehavior")]
    public class DashBehavior : Behavior, IBehaviorUseListener
    {
        [Header("Dash Settings")]
        public float dashSpeed = 40f;
        public float dashDuration = 0.2f;

        public bool Use(CardEnv env, Agent agent)
        {
            var player = OpenWorldEnv.Current.PlayerTransform;
            if (player == null) 
            {
                Debug.LogWarning("DashBehavior: PlayerSingleton.Instance is null");
                return false;
            }

            // Get or add runtime dash controller
            PlayerDashController dashController = player.GetComponent<PlayerDashController>();
            if (dashController == null)
            {
                dashController = player.gameObject.AddComponent<PlayerDashController>();
            }

            // Trigger dash — direction comes from keyboard input inside controller
            dashController.StartDash(dashSpeed, dashDuration);

            return true;
        }
    }
}