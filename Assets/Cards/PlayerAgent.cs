using Cards.Core;

namespace Cards
{
    public class PlayerAgent: Agent
    {
        public Card mainHandCard;
        public Card offHandCard;
        
        /*
         * Returns in order: main/off/other
         */
        public override Card SelectCardImmediate()
        {
            return RandomCard;
        }
    }
}