using UnityEngine;

namespace Characters.Interactables
{
    public class Interactable : MonoBehaviour
    {
        // Determines if this object can currently be interacted with
        public bool canInteract = true;
        public float interactTime = 0;
        public string interactText = "(E) Interact";

        [Header("Audio")]
        public AudioClip interactSound; // assign in inspector, optional
        public bool playSoundOnStart = false;

        public virtual void Interact(GameObject player)
        {   
            if (!canInteract)
                return;

            // Only play on completion if the flag is NOT set
            if (!playSoundOnStart)
            {
                PlaySound();
            }
        }


        public void PlaySound()
        {
            if (interactSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(
                    interactSound,
                    transform.position,
                    1f
                );
            }
        }

    }
}
