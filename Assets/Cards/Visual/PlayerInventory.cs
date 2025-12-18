using Characters;
using UnityEngine;

namespace Cards.Visual
{
    public class PlayerInventory : CardInventory
    {
        private PlayerAgent Player => (PlayerAgent)targetAgent;
        protected override void Update()
        {
            Player.SetMainHand(handLayout.SelectedCard?.AttachedCard);
            base.Update();
        }
    }
}