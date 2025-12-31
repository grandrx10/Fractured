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
            GlobalWorldManager.Instance.Transition(domainName, transform.position, domainPoint, domainTag);
        }
    }
}
