using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards.Core
{
    public class BaseCardContainer : MonoBehaviour
    {
        public Image icon;
        public TextMeshProUGUI title, description;
        public TextMeshProUGUI hp, mp, dmg;
        public Image background;
        public List<Color> rarityColors;
        public void SetBg(CardRarity rarity)
        {
            background.color = rarityColors[(int) rarity];
        }
    }
}