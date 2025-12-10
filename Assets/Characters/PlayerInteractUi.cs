using UnityEngine;
using TMPro;

public class PlayerInteractUi : MonoBehaviour
{
    private PlayerInteractController playerInteract;
    private TextMeshProUGUI interactText;

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
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }
}
