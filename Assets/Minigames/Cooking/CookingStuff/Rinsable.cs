using UnityEngine;

namespace Minigames.Cooking.CookingStuff
{
    public class Rinsable : MonoBehaviour
    {
        public bool rinsed = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Rinse()
        {
            rinsed = true;
        }
    }
}
