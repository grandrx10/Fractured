namespace Characters.Dialogue
{
    public class FollowEvent : DialogueEvent
    {
        public Follower follower;
        public Followable followable;
        public override void Execute()
        {
            follower.SetFollowable(followable);
        }
    }
}