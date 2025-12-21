using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards.Environments
{
    public enum CardSubmitState
    {
        Invalid,
        Success,
        Failure
    }

    // We could probably replace this with some interfaces
    // Currently the expectation is that cards type check the CardEnv parameter to execute behavior
    public class CardEnv: MonoBehaviour
    {
        public float environmentExitTime;
        public float environmentIntroTime;
        public float environmentIntroRad;
        [SerializeField] private List<Transform> environmentCenters;
        private void Start()
        {
            GlobalWorldManager.Instance.Load(this);
        }

        public Transform GetEnvCenter(string n)
        {
            var t = environmentCenters.Find(x => x.name == n);
            return t;
        }

        public virtual void Initialize(PlayerAgent player)
        {
            throw new System.NotImplementedException();
        }
        
        public virtual void Destroy()
        {
            throw new System.NotImplementedException();
        }

        private void OnDrawGizmos()
        {
            foreach (var v in environmentCenters)
            {
                Gizmos.DrawWireSphere(v.position, environmentIntroRad);
            }
        }
    }
}