using Cards.Core.BehaviorTags;
using UnityEngine;

namespace Cards.Core.Behaviors
{
    [CreateAssetMenu(fileName = "Card Game", menuName = "Behaviors/DefaultCardGame")]
    public class DefaultCardGameBehavior: Behavior, IBehaviorTurnListener
    {
        public virtual (int, int) Collide(Card opponent)
        {
            Debug.Log("the card attacking");
            
            return (0, 0);
        }
    }
}