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

        // Static cached audio clip for all TriggerDomainEvent instances
        private static AudioClip domainExpansionClip;

        private void Awake()
        {
            // Load the domain expansion clip once
            if (domainExpansionClip == null)
            {
                domainExpansionClip = Resources.Load<AudioClip>("Audio/domainExpansion");
                if (domainExpansionClip == null)
                {
                    Debug.LogWarning("TriggerDomainEvent: domainExpansion clip not found in Resources/Audio!");
                }
            }
        }

        [ContextMenu("Execute")]
        public override void Execute()
        {
            // Play the domain expansion sound first
            if (domainExpansionClip != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(
                    domainExpansionClip,
                    transform.position,
                    1f
                );
            }

            // Safety check
            if (domain == null)
            {
                Debug.LogWarning("TriggerDomainEvent: No DomainTrigger assigned.");
                return;
            }

            // Move domain trigger to player's feet if requested
            if (TriggerAtPlayerFeet)
            {
                if (OpenWorldEnv.Current == null || OpenWorldEnv.Current.player == null)
                {
                    Debug.LogWarning("TriggerDomainEvent: OpenWorldEnv or player not found.");
                }
                else
                {
                    Transform playerTransform = OpenWorldEnv.Current.player.transform;
                    Vector3 feetPos = playerTransform.position - new Vector3(0f, 2f, 0f);
                    domain.transform.position = feetPos;
                }
            }

            // Trigger the domain event
            domain.Trigger();
        }
    }
}
