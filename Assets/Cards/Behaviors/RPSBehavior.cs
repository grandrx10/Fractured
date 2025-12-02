using Cards.Core.Behaviors;
using Cards.Environments;
using UnityEngine;

namespace Cards.Behaviors
{
    [CreateAssetMenu(fileName = "RPS", menuName = "Behaviors/RPSBehavior")]
    public class RPSBehavior: BaseBehavior
    {
        public RPSType type;
        
        public enum RPSType
        {
            R,
            P,
            S,
            Rp,
            Pp,
            Sp
        }
    }
}