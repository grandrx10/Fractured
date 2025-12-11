using Characters;
using Characters.Player;

namespace Cards.Core.BehaviorTags
{
    public interface IBehaviorStatUpdater
    {
        public PlayerStats GetStats();
    }
}