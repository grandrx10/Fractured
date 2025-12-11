using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.RPS.B
{
    [CreateAssetMenu(fileName = "Scissors", menuName = "Behaviors/Scissors")]
    public class ScissorsBehavior: Behavior
    {
        public override string GetDescription()
        {
            return $"<b>Snip Snip</b>: On Hit, [Cut](Cuts) twice.";
        }
    }
}