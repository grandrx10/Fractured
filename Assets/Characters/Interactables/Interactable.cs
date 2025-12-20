using UnityEngine;

namespace Characters.Interactables
{
    public class Interactable : MonoBehaviour
    {
        // public string interactName = "";
        public bool canInteract = true;
        public float interactTime = 0;

        public virtual void Interact(GameObject player)
        {
        
        }
    }
}
