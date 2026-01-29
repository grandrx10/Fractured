using UnityEngine;
using World.Objects;

namespace World.Domain
{
    public class DomainTrigger : MonoBehaviour, IDomainTriggerable
    {
        public string domainName;
        public string domainPoint;

        [Header("Position")]
        public bool atPlayer;
        public Vector3 offset;
        public bool offsetLocal;

        [Header("Metadata")]
        [TextArea] public string domainTag;

        public void Trigger()
        {
            Trigger(GetStartPoint());
        }

        public void TriggerInteract(BaseInteractable __, GameObject _)
        {
            Trigger();
        }

        public void Trigger(Vector3 startPoint)
        {
            GlobalWorldManager.Instance.Transition(
                domainName,
                startPoint,
                domainPoint,
                domainTag
            );
        }

        private Vector3 GetStartPoint()
        {
            Transform baseTransform = transform;

            if (atPlayer)
            {
                var player = GlobalWorldManager.Instance.PlayerAgent;
                baseTransform = player.transform;
            }

            if (offsetLocal)
            {
                return baseTransform.position + baseTransform.rotation * offset;
            }

            return baseTransform.position + offset;
        }
    }
}