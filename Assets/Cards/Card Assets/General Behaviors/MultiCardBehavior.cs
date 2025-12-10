using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Cards.Behaviors
{
    [CreateAssetMenu(fileName = "MultiCard", menuName = "Behaviors/MultiCard")]
    public class MultiCardBehavior : Behavior, IBehaviorUseListener
    {
        public float angle = 30f; // total spread in degrees
        public int count = 3;     // number of cards to throw
        public TextMeshProUGUI menuDesc;

        public void Use(CardEnv env, Agent agent)
        {
            if (env is OpenWorldEnv opEnv)
            {
                if (count <= 0)
                {
                    Debug.LogWarning("Count must be >= 1");
                    count = 1;
                }

                // Compute the angle step
                float step = (count > 1) ? angle / (count - 1) : 0f;
                float startAngle = -angle / 2f;

                for (int i = 0; i < count; i++)
                {
                    float currentAngle = startAngle + i * step;
                    Quaternion rot = Quaternion.Euler(0f, currentAngle, 0f);
                    opEnv.ThrowCard(AttachedCard, rot, 30); // 30 can be replaced with a speed parameter
                }
            }
            else
            {
                Debug.LogError("Env does not support throwing");
            }
        }
        
        public override string GetDescription()
        {
            return $"<b>Multishot</b>: Throws {count} cards.";
        }

        public override GameObject GetMenuObject()
        {
            var o = Instantiate(menuDesc);
            o.text = $"Multishot: Throws {count} cards.";
            return o.gameObject;
        }
    }
}