using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "Use", menuName = "Behaviors/Use")]
    public class NamedUseBehavior: DefaultUseBehavior
    {
        public string abilityName = "Throw";
        public string objectName = "card";
        
        public override string GetDescription(Card card)
        {
            return $"<b>(Active) {abilityName}</b>: Throw {TextHelper.WithIndefiniteArticle(objectName)}.";
        }
    }
}