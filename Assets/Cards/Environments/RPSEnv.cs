using System;
using Cards.Card_Assets.RPS.B;
using Cards.Core;
using Cards.Core.Util;
using UnityEngine;

namespace Cards.Environments
{
    public class RPSEnv: CardEnv
    {
        private Agent _agent1, _agent2;
        
        [ContextMenu("test")]
        private void TestRps()
        {
            var objs = FindObjectsByType<Agent>(FindObjectsSortMode.None);

            if (objs.Length >= 2)
            {
                Initialize(objs[0], objs[1]);
            }
        }
        
        public void Initialize(Agent agent1, Agent agent2)
        {
            _agent1 = agent1;
            _agent2 = agent2;
            var cb = new CallbackWaiter2<Card>(RPS);
            Debug.Log("RPS Initialized");
            agent1.SelectCardAsync(cb.SetA, 1);
            agent2.SelectCardAsync(cb.SetB, 1);
        }

        public void RPS(Card card1, Card card2)
        {
            var val1 = card1.TryGetBehavior(out RPSBehavior behavior1);
            var val2 = card2.TryGetBehavior(out RPSBehavior behavior2);
            if (val1 && val2)
            {
                Debug.Log("we are playing rps");
            }
            else
            {
                Debug.Log("rps failed");
            }
        }
    }
}