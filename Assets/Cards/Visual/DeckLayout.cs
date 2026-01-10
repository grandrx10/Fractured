using System;
using UnityEngine;
using System.Collections.Generic;
using Cards.Core;
using Cards.Core.Behaviors;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

namespace Cards.Visual
{
    public class DeckLayout : CardInteractionContainer
    {

        public override void OnCardClick(CardDisplayInteractable card)
        {
            RefreshLayout();
        }

        // public override void RefreshLayout()
        // {
        //     ValidateSelectedCard();
        //     foreach (Transform child in infoCard) Destroy(child.gameObject);
        //     
        //     if (_selectedCard)
        //     {
        //         var v = _selectedCard.AttachedCard.Visuals;
        //         infoCardName.text = v.Name;
        //         infoCardColl.text = v.CollectionName;
        //         
        //         var cc = Instantiate(cardPrefab, content);
        //         cc.transform.localScale *= 1.3f;
        //         cc.interactable = true;
        //         cc.card = _selectedCard.AttachedCard;
        //         cc.transform.SetParent(infoCard, false);
        //         
        //         foreach (var behavior in _selectedCard.AttachedCard.GetAllBehaviors<Behavior>())
        //         {
        //             var desc = behavior.GetMenuObject();
        //             if (desc != null)
        //             {
        //                 desc.transform.SetParent(infoCard);
        //             }
        //         }
        //     }
        //     else
        //     {
        //         infoCardName.text = "";
        //         infoCardColl.text = "";
        //     }
        // }

        private void ValidateSelectedCard()
        {
            //if (_selectedCard && !cards.Contains(_selectedCard)) _selectedCard = null;
        }
        
        public override bool OnCardDropped(CardInteractionContainer source, CardDisplayInteractable card)
        {
            if (card.AttachedCard.IsTarot) return false;
            if (source.OnCardRemoved(card))
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    content,
                    Input.mousePosition,
                    UIHelper.UICamera,
                    out var pointerLocal
                );
                    
                int targetIndex = GetGridInsertIndexLocal(
                    content.GetComponent<GridLayoutGroup>(),
                    content, 
                    pointerLocal,
                    CardDisplays.Count);
                
                AddCard(card.AttachedCard);
                AddCardDisplay(card.AttachedCard, targetIndex);
                RefreshLayout();
                return true;
            }
            return false;
        }
        
        public static int GetGridInsertIndexLocal(
            GridLayoutGroup grid,
            RectTransform container,
            Vector2 pointerLocal,
            int itemCount
        )
        {
            Rect rect = container.rect;

            // Convert to top-left origin
            float x = pointerLocal.x + rect.width * 0.5f;
            float y = -pointerLocal.y;

            // Subtract padding
            x -= grid.padding.left;
            y -= grid.padding.top;

            // Clamp inside content
            x = Mathf.Max(0, x);
            y = Mathf.Max(0, y);

            float stepX = grid.cellSize.x + grid.spacing.x;
            float stepY = grid.cellSize.y + grid.spacing.y;

            int col, row, index;

            switch (grid.constraint)
            {
                case GridLayoutGroup.Constraint.FixedColumnCount:
                {
                    int cols = grid.constraintCount;

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    col = Mathf.Clamp(col, 0, cols - 1);

                    index = row * cols + col;
                    break;
                }

                case GridLayoutGroup.Constraint.FixedRowCount:
                {
                    int rows = grid.constraintCount;

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    row = Mathf.Clamp(row, 0, rows - 1);

                    index = col * rows + row;
                    break;
                }

                default: // Flexible
                {
                    int cols = Mathf.Max(
                        1,
                        Mathf.FloorToInt(
                            (rect.width - grid.padding.horizontal + grid.spacing.x) / stepX
                        )
                    );

                    col = Mathf.FloorToInt(x / stepX);
                    row = Mathf.FloorToInt(y / stepY);

                    col = Mathf.Clamp(col, 0, cols - 1);

                    index = row * cols + col;
                    //Debug.Log($"{col} {row} {index} {pointerLocal.x}/{x}/{stepX} {pointerLocal.y}/{y}/{stepY}");
                    break;
                }
            }

            // Clamp to insertion range
            return Mathf.Clamp(index, 0, itemCount);
        }
        
        public override void AddCardDisplay(Card card, int position=-1)
        {
            if (card.IsTarot) return;
            if (position == -1) position = CardDisplays.Count - 1;
            base.AddCardDisplay(card, position);
        }
    }
}