using Cards.Core.Behaviors;
using Cards.Environments;
using UnityEngine;

namespace Cards.Behaviors
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