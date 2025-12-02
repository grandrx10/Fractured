using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Environments
{
    public class OpenWorldEnv: MonoBehaviour
    {
        public Agent player;
        public PhysicalCard cardPrefab;
        public float mana;
        public void Awake()
        {
            player.SelectCardAsync(UseCard, -1);
        }

        public CardSubmitState UseCard(Card card)
        {
            card.TryGetBehavior(out BaseUseBehavior useBehavior);
            var pc = useBehavior.Throw(cardPrefab);
            return CardSubmitState.Success;
        }
    }
}