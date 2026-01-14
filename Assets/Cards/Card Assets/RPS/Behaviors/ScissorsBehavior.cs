using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.RPS.Behaviors
{
    [CreateAssetMenu(fileName = "Scissors", menuName = "Behaviors/Scissors")]
    public class ScissorsBehavior: DefaultUseBehavior
    {
        public override string GetDescription(Card card)
        {
            return $"<b>Snip Snip</b>: On Hit, [Cut](Cuts) twice.";
        }
    }
}