using System.Collections.Generic;
using UnityEngine;
using Game;

namespace Utils
{
    public class SetTrigger : MonoBehaviour
    {
        public List<string> preconditions = new();
        
        public enum ConditionMode
        {
            And,
            Or,
            Not
        }

        public ConditionMode conditionMode = ConditionMode.And;
        public string target;

        public void Activate()
        {
            if (!CheckPreconditions())
                return;

            // Only add if it doesn't already exist (optional safety)
            if (!GlobalState.instance.HasEvent(target))
            {
                GlobalState.instance.AddEvent(target);
            }
        }

        private bool CheckPreconditions()
        {
            if (preconditions == null || preconditions.Count == 0)
                return true;

            switch (conditionMode)
            {
                case ConditionMode.And:
                    foreach (var condition in preconditions)
                    {
                        if (!GlobalState.instance.HasEvent(condition))
                            return false;
                    }
                    return true;

                case ConditionMode.Or:
                    foreach (var condition in preconditions)
                    {
                        if (GlobalState.instance.HasEvent(condition))
                            return true;
                    }
                    return false;

                case ConditionMode.Not:
                    foreach (var condition in preconditions)
                    {
                        if (GlobalState.instance.HasEvent(condition))
                            return false;
                    }
                    return true;

                default:
                    return false;
            }
        }
    }
}