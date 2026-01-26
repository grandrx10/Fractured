using System.Collections.Generic;
using Cards.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards.Visual
{
    public class CardDisplayPrefab : MonoBehaviour
    {
        public Image icon;
        [SerializeField] private TextMeshProUGUI title, flavor;
        public TextMeshProUGUI hp, mp, dmg;
        public TextMeshProUGUI description;
        public virtual void SetDesc(string s)
        {
            description.text = s;
        }

        public void SetTitle(string s)
        {
            title.text = s;
        }

        public void SetFlavor(string s)
        {
            flavor.text = s;
        }
    }
}