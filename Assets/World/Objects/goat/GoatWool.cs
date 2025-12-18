using System;
using Cards;
using Cards.Card_Assets.RPS.Behaviors;
using Cards.Core;
using Game;
using UnityEngine;

namespace World.Objects.goat
{
    public class GoatWool : MonoBehaviour, ICuttable
    {
        public string goatId;
        public Animator animator;
        public bool cut;
        public CardData c;
        public void Start()
        {
            if (goatId != "" && GlobalState.instance.HasEvent($"GOAT_{goatId}_SHEAR"))
            {
                animator.Play("Cut", -1, 1);
                cut = true;
            }
        }

        public void Cut(Vector3 cutNormal, Vector3 cutPosition)
        {
            if (cut) return;
            cut = true;
            animator.Play("Cut");
            
            FindFirstObjectByType<PlayerAgent>().AddCard(Card.MakeCard(c));
            
            if (goatId != "")
            {
                GlobalState.instance.AddEvent($"GOAT_{goatId}_SHEAR");
            }
        }
    }
}