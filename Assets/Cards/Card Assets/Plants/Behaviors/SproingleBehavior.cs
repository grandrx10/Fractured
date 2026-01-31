using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters.Player;
using UnityEngine;

namespace Cards.Card_Assets.Plants.Behaviors
{
    [CreateAssetMenu(fileName = "Sproingle", menuName = "Behaviors/Sproingle")]
    public class SproingleBehavior : Behavior, IBehaviorUseListener
    {
        public float force;
        public override string GetDescription(Card card)
        {
            return $"<b>(Active) Sproing</b>: Leap into the air.";
        }

        public bool Use(Card card, CardEnv env, Agent agent)
        {
            var rb = agent.GetComponent<Rigidbody>();
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = Vector3.Scale(rb.linearVelocity, new Vector3(1, 0.5f, 1));
            }
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            return true;
        }
    }
}