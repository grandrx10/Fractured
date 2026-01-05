using Cards.Core.Behaviors;
using Game;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Quests", menuName = "Behaviors/Quests")]
    public class QuestsBehavior : Behavior
    {
        public override string GetDescription()
        {
            string s = "";
            foreach (var qst in GlobalState.instance.quests)
            {
                string txt = qst.Value;
                if (qst.Value.StartsWith("(DONE) "))
                {
                    txt = $"<s>{txt.Substring(7)}</s>";
                }
                s += $"- {txt}\n";
            }
            return s;
        }
    }
}