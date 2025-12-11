using UnityEngine;

namespace Characters.Interactables
{
    public class Interactable : MonoBehaviour
    {
        public bool canInteract = true;

        public virtual void Interact(GameObject player)
        {
        
        }
    }
}
