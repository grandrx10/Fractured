using Cards.Core.Behaviors;
using UnityEngine;

namespace Cards.Card_Assets.Systems.B
{
    [CreateAssetMenu(fileName = "Wallet", menuName = "Behaviors/Wallet")]
    public class WalletBehavior : Behavior
    {
        public int money;
    }
}