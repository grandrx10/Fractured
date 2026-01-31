using Cards;
using Cards.Core;
using Cards.Environments;
using Game;
using Game.Effects;
using UnityEngine;
using Utils;
using World.Objects;

public class SpringberryBush : MonoBehaviour
{
    public float cd;
    public GameObject berries;

    [Header("Audio")]
    public AudioClip eatSound; // assign in inspector

    public void Interact(BaseInteractable I, GameObject player)
    {

        // Play eat sound immediately
        if (eatSound != null)
        {
            AudioManager.Instance.PlayOneShot(
                eatSound,
                transform.position,
                1f
            );
        }

        OpenWorldEnv.Current.AddEffect<SpringEffect>();
        berries.SetActive(false);
        I.canInteract = false;

        // Re-enable berries after cooldown
        Delay.Call(this, cd, () =>
        {
            berries.SetActive(true);
            I.canInteract = true;
        });
        
    }
}
