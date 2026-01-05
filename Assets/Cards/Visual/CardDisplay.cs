using System;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils;

namespace Cards.Visual
{
    public class CardDisplay : MonoBehaviour, IPointerClickHandler
    {
        public Card card;
        public List<CardDisplayPrefab> cardContainers;
        public bool interactable, hasDepth, noPreview;
        private int _selectedLink;
        [FormerlySerializedAs("m_TextPopup_RectTransform")] public RectTransform mTextPopupRectTransform;
        [FormerlySerializedAs("m_TextPopup_TMPComponent")] public TextMeshProUGUI mTextPopupTMPComponent;
        public CardPreview preview;
        private CardDisplayPrefab _cc;
        private CardPreview _currentPreview;
        public Action DisplayClicked;
        void Start()
        {
            var v = card.Visuals;
            var container = cardContainers[(int)v.Style];
            if (card.GetData() is TarotCardData tarotData) container = tarotData.customDisplay;
            _cc = Instantiate(container, transform);
            _cc.transform.SetAsFirstSibling();
            _cc.transform.localPosition = Vector3.zero;
            _cc.icon.sprite = v.Icon;
            _cc.SetTitle($"{v.Name}");
            _cc.hp.text = $"{card.stats.health}";
            _cc.mp.text = $"{card.stats.mana}";
            _cc.dmg.text = $"{card.stats.strength}";
            _cc.SetFlavor(v.FlavorText);
            _cc.SetDesc(MakeDescription());
            if (!noPreview) {
                DisplayClicked += () =>
                {
                    _currentPreview = CreatePreview(card);
                };
                
            }
            if (!hasDepth)
            {
                UIHelper.FlattenChildrenZ(_cc.transform);
            }
        }

        private void OnDisable()
        {
            if (_currentPreview) Destroy(_currentPreview.gameObject);
        }

        public CardPreview CreatePreview(Card c)
        {
            var cardPrev = Instantiate(preview, UIHelper.GetRootCanvas(transform).transform);
            cardPrev.cardDisplay.card = c;
            cardPrev.cardDisplay.interactable = true;
            cardPrev.cardDisplay.hasDepth = true;
            return cardPrev;
        }

        private void LateUpdate()
        {
            if (interactable)
            {
                Camera c = GameObject.FindGameObjectWithTag("UI Camera").GetComponent<Camera>();
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(_cc.description, Input.mousePosition, c);

                if ((linkIndex == -1 && _selectedLink != -1) || linkIndex != _selectedLink)
                {
                    mTextPopupRectTransform.gameObject.SetActive(false);
                    _selectedLink = -1;
                }

                // Handle new Link selection.
                if (linkIndex != -1 && linkIndex != _selectedLink)
                {
                    _selectedLink = linkIndex;

                    TMP_LinkInfo linkInfo = _cc.description.textInfo.linkInfo[linkIndex];

                    //Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\"");

                    Vector3 worldPointInRectangle;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(_cc.description.rectTransform, Input.mousePosition, c, out worldPointInRectangle);
                    
                    mTextPopupRectTransform.position = worldPointInRectangle;
                    mTextPopupRectTransform.gameObject.SetActive(true);
                    mTextPopupTMPComponent.text = AbilityDescriptions.GetDescription(linkInfo.GetLinkID());
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

