using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.RPS.B
{
    [CreateAssetMenu(fileName = "RPS", menuName = "Behaviors/RPS")]
    public class RPSBehavior: Behavior
    {
        public RPSType type;
        [Range(0, 2)] public int tier;
        public enum RPSType
        {
            R,
            P,
            S,
        }
        
        public override string GetDescription()
        {
            return $"<b>{GetName()}</b>: Beats {GetCounterName()} at RPS";
        }

        private string GetName()
        {
            return type == RPSType.R ? "Rock" : (type == RPSType.P ? "Paper" : "Scissors");
        }
        
        private string GetCounterName()
        {
            return type == RPSType.R ? "Scissors" : (type == RPSType.P ? "Rock" : "Paper");
        }
    }
}