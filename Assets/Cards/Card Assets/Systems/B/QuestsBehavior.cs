using Cards.Core;
using Cards.Core.Behaviors;
using Game;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Quests", menuName = "Behaviors/Quests")]
    public class QuestsBehavior : Behavior
    {
        public override string GetDescription(Card card)
        {
            string s = "";
            foreach (var qst in GlobalState.instance.quests)
            {
                var txt = qst.Value;
                string q = txt.Item1;
                if (txt.Item2)
                {
                    q = $"<s>{q}</s>";
                }
                s += $"- {q}\n";
            }
            return s;
        }
    }
}