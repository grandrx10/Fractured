using UnityEngine;
using World.Domain;
using Cards.Environments;

namespace Characters.Dialogue
{
    public class TriggerDomainEvent : DialogueEvent
    {
        [Header("Domain Trigger")]
        public DomainTrigger domain;

        [Header("Options")]
        public bool TriggerAtPlayerFeet = false;
        
        [ContextMenu("Execute")]
        public override void Execute()
        {
            if (domain == null)
            {
                Debug.LogWarning("TriggerDomainEvent: No DomainTrigger assigned.");
                return;
            }

            if (TriggerAtPlayerFeet)
            {
                if (OpenWorldEnv.Current == null || OpenWorldEnv.Current.player == null)
                {
                    Debug.LogWarning("TriggerDomainEvent: OpenWorldEnv or player not found.");
                }
                else
                {
                    Transform playerTransform = OpenWorldEnv.Current.player.transform;

                    // Move domain trigger to player's feet (ground-level position)
                    Vector3 feetPos = playerTransform.position - new Vector3(0f, 2f, 0f);
                    domain.transform.position = feetPos;
                }
            }

            domain.Trigger();
        }
    }
}
