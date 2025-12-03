using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Environments
{
    public class OpenWorldEnv: CardEnv
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
            useBehavior.Throw(this, player);
            return CardSubmitState.Success;
        }

        public void ThrowCard(Card card, Quaternion rotation, float speed)
        {
            var p = player.transform.position;
            var d = rotation * player.transform.forward * speed;
            var c = Instantiate(cardPrefab, p, Quaternion.LookRotation(d));
            c.card = card;
            c.GetComponent<Rigidbody>().linearVelocity = d;
        }
    }
}