using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cards.Visual
{
    public class CardDisplay : MonoBehaviour, IPointerClickHandler
    {
        public Card card;
        public List<CardDisplayPrefab> cardContainers;
        public bool interactable;
        private int _selectedLink;
        public RectTransform m_TextPopup_RectTransform;
        public TextMeshProUGUI m_TextPopup_TMPComponent;
        private CardDisplayPrefab _cc;

        public Action DisplayClicked;
        void Start()
        {
            var v = card.Visuals;
            var container = cardContainers[(int)v.Style];
            _cc = Instantiate(container, transform);
            _cc.transform.SetAsFirstSibling();
            _cc.transform.localPosition = Vector3.zero;
            _cc.icon.sprite = v.Icon;
            _cc.title.text = $"{v.Name}";
            _cc.hp.text = $"{card.stats.health}";
            _cc.mp.text = $"{card.stats.mana}";
            _cc.dmg.text = $"{card.stats.strength}";
            _cc.flavor.text = v.FlavorText;
            _cc.description.text = MakeDescription();
            _cc.SetBg(v.Rarity);
        }

        private void LateUpdate()
        {
            if (interactable)
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(_cc.description, Input.mousePosition, null);

                if ((linkIndex == -1 && _selectedLink != -1) || linkIndex != _selectedLink)
                {
                    m_TextPopup_RectTransform.gameObject.SetActive(false);
                    _selectedLink = -1;
                }

                // Handle new Link selection.
                if (linkIndex != -1 && linkIndex != _selectedLink)
                {
                    _selectedLink = linkIndex;

                    TMP_LinkInfo linkInfo = _cc.description.textInfo.linkInfo[linkIndex];

                    //Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\"");

                    Vector3 worldPointInRectangle;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(_cc.description.rectTransform, Input.mousePosition, null, out worldPointInRectangle);
                    
                    m_TextPopup_RectTransform.position = worldPointInRectangle;
                    m_TextPopup_RectTransform.gameObject.SetActive(true);
                    m_TextPopup_TMPComponent.text = AbilityDescriptions.GetDescription(linkInfo.GetLinkID());
                }
            }
        }

        private string MakeDescription()
        {
            string t = "";
            foreach (var behavior in card.GetAllBehaviors<Behavior>())
            {
                string s = AbilityDescriptions.ConvertBracketLinks(behavior.GetDescription());
                if (s=="") continue;
                t += s + "\n";
            }

            return t;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DisplayClicked?.Invoke();
        }
    }
}

