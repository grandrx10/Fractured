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
    public void Interact(BaseInteractable I, GameObject player, bool init)
    {
        if (!init)
        {
            OpenWorldEnv.Current.AddEffect<SpringEffect>();
            berries.SetActive(false);
            I.canInteract = false;
            Delay.Call(this, cd, () =>
            {
                berries.SetActive(true);
                I.canInteract = true;
            });
        }
    }
}
