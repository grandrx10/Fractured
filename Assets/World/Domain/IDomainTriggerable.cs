using UnityEngine;
using World.Objects;

namespace World.Domain
{
    public interface IDomainTriggerable
    {
        public void Trigger();
        
        public void Trigger(Vector3 startPoint);
    }
}
