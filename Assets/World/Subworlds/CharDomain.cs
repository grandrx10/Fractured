using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.Subworlds
{
    public class CharDomain: MonoBehaviour
    {
        public List<GameObject> characters;
        private void Awake()
        {
            var t = GlobalWorldManager.Instance.TransitionTag;
            if (t.StartsWith("char:"))
            {
                foreach (var c in characters)
                {
                    if (t.Contains(c.name))
                    {
                        c.SetActive(true);
                    }
                    else
                    {
                        c.SetActive(false);
                    }
                }
            }
        }
    }
}