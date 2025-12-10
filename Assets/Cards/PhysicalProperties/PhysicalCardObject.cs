using System;
using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalCardObject : MonoBehaviour
    {
        public PhysicalCardInitState InitState;
        public Action<PhysicalCardInitState> OnInit;
        
        public struct PhysicalCardInitState
        {
            public Vector3 StartPosition;
            public Vector3 CenterPosition;
            public Vector3 TargetDirection;
            public Vector3 StartDirection;
            public Transform Target;
            public float Speed;
        }
        
        public virtual void Start()
        {
            OnInit?.Invoke(InitState);
        }
    }
}