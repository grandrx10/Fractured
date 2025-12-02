using Cards.Core;
using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Visual
{
    public class BaseCardDisplay : MonoBehaviour
    {
        public Card card;
        public BaseCardContainer cardContainer;
    
        void Start()
        {
            var v = card.Visuals;
            var cc = Instantiate(cardContainer, transform);
            cc.transform.localPosition = Vector3.zero;
            cc.icon.sprite = v.Icon;
            cc.title.text = $"{v.Name}";
            cc.hp.text = $"{card.stats.health}";
            cc.mp.text = $"{card.stats.mana}";
            cc.dmg.text = $"{card.stats.strength}";
            cc.flavor.text = v.FlavorText;
            cc.description.text = MakeDescription();
            cc.SetBg(v.Rarity);
        }

        private string MakeDescription()
        {
            string t = "";
            foreach (var behavior in card.GetAllBehaviors<BaseBehavior>())
            {
                string s = behavior.GetDescription();
                if (s=="") continue;
                t += s + "\n";
            }

            return t;
        }
    }
}

