using UnityEngine;
using World.Objects;

namespace World.Domain
{
    public class DomainTrigger : MonoBehaviour
    {
        public string domainName, domainPoint;
        [TextArea] public string domainTag;

        public void Trigger()
        {
            Trigger(transform.position);
        }
        
        public void TriggerInteract(BaseInteractable __, GameObject _, bool init)
        {
            if (!init) Trigger(transform.position);
        }
        
        public void Trigger(Vector3 startPoint)
        {
            GlobalWorldManager.Instance.Transition(domainName, startPoint, domainPoint, domainTag);
        }
    }
}
