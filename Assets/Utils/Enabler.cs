using UnityEngine;

namespace Utils
{
    public class Enabler : MonoBehaviour
    {
        public GameObject target;

        public void Enable(bool b)
        {
            target.SetActive(b);
        }
    }
}