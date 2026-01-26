using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Environments;
using TMPro;
using UnityEngine;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "MultiCard", menuName = "Behaviors/MultiCard")]
    public class MultiCardBehavior : DefaultUseBehavior
    {
        public float angle = 30f; // total spread in degrees
        public int count = 3;     // number of cards to throw
        public string objectName = "cards";
        
        public override bool Use(Card card, CardEnv env, Agent agent)
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
                    ThrowCard(card, agent, opEnv, rot); // 30 can be replaced with a speed parameter
                }
            }
            else
            {
                Debug.LogError("Env does not support throwing");
            }

            return true;
        }
        
        public override string GetDescription(Card card)
        {
            return $"<b>(Active) Multishot</b>: Throw {count} {objectName}.";
        }

        //public override GameObject GetMenuObject()
        //{
        //    var o = Instantiate(menuDesc);
        //    o.text = $"Multishot: On Use, Throws {count} {objectName}.";
        //    return o.gameObject;
        //}
    }
}