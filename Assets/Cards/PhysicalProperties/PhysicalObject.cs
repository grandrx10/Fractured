using System;
using Cards.Core;
using UnityEngine;

namespace Cards.PhysicalProperties
{
    public class PhysicalObject : MonoBehaviour
    {
        public PhysicalInitState InitState;
        public Action<PhysicalInitState> OnInit;
        public Action<PhysicalActiveState> OnMove;
        public Card card;
        
        public struct PhysicalInitState
        {
            public Vector3 StartPosition;
            public Vector3 CenterPosition;
            public Vector3 TargetDirection;
            public Vector3 StartDirection;
            public Transform Target;
            public float Speed;
        }
        
        public struct PhysicalActiveState
        {
            public Vector3 StartPosition;
            public Vector3 StartDirection;
            public float Speed;
        }
        
        public virtual void Start()
        {
            OnInit?.Invoke(InitState);
        }

        public virtual void Move(PhysicalActiveState state)
        {
            OnMove?.Invoke(state);
        }
    }
}