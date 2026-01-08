using System;
using Cards.Core;
using Cards.Environments;
using UnityEngine;

namespace Cards
{
    public class EnvCardAssign : MonoBehaviour
    {
        public CardEnv env;
        public Card card;
        private void Awake()
        {
            env.OnLoad += () =>
            {
                env.player.AddCard(card, true);
            };
        }
    }
}