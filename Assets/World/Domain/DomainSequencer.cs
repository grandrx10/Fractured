using UnityEngine;
using World.Objects;

namespace World.Domain
{
    public class DomainSequencer : MonoBehaviour
    {
        public string domainName, domainPoint;

        public void Interact(BaseInteractable I, GameObject player, bool b)
        {
            if (!b)
            {
                GlobalWorldManager.Instance.Transition(domainName, transform.position, domainPoint);
            }
        }
    }
}
