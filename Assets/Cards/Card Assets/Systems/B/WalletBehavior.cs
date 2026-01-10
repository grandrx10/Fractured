using Cards.Core.Behaviors;
using Game;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Wallet", menuName = "Behaviors/Wallet")]
    public class WalletBehavior : Behavior
    {
        public override string GetDescription()
        {
            string s = $"Coins: {GlobalState.instance.Money}";
            return s;
        }
    }
}