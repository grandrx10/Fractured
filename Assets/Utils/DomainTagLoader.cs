using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class DomainTagLoader: MonoBehaviour
    {
        public List<GameObject> objects;
        private void Awake()
        {
            var t = GlobalWorldManager.Instance.TransitionTag;
            foreach (var c in objects)
            {
                c.SetActive(t.Contains(c.name));
            }
        }
    }
}