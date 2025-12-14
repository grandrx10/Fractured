using TMPro;
using UnityEngine;

namespace Characters
{
    public class PlayerInteractUi : MonoBehaviour
    {
        private PlayerInteractController playerInteract;
        private TextMeshProUGUI interactText;
        private TextMeshProUGUI interactableName;

        void Start()
        {
            playerInteract = GetComponentInParent<PlayerInteractController>();

            // Find TMP on this object
            interactText = GetComponent<TextMeshProUGUI>();

            // Hide text at start
            interactText.gameObject.SetActive(false);

        }

        void Update()
        {
            if (playerInteract != null && playerInteract.currentInteractable != null)
            {
                interactText.gameObject.SetActive(true);
                if (playerInteract.currentInteractable.interactName != "")
                {
                    interactableName.text = playerInteract.currentInteractable.interactName;
                }
                
                interactableName.gameObject.SetActive(true);
            }
            else
            {
                interactText.gameObject.SetActive(false);
                interactableName.gameObject.SetActive(false);
            }
        }
    }
}
