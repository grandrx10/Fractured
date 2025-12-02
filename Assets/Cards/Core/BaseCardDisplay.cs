using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards.Core
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
            cc.SetBg(v.Rarity);
        }
    }
}

