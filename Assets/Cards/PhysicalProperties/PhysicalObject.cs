using System;
using Cards.Core;
using UnityEngine;
using Utils;

namespace Cards.PhysicalProperties
{
    public class PhysicalObject : MonoBehaviour
    {
        public PhysicalInitState InitState;
        public Action<PhysicalInitState> OnInit;
        public Action<PhysicalActiveState> OnMove;
        public Action<PhysicalHitState> OnHit;
        public Card card;
        public LayerMask hitLayers;
        protected Rigidbody rb;
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
        
        public struct PhysicalHitState
        {
            public Collision Other;
            public Vector3 Position;
            public Vector3 Velocity;
            public Vector3 Direction;
            public float Speed;
        }
        
        public virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            OnInit?.Invoke(InitState);
        }
        
        Vector3 _preCollisionVelocity;

        void FixedUpdate()
        {
            if (!rb) return;
            _preCollisionVelocity = rb.linearVelocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            var g = PhysicsHelper.MainObj(other.collider);
            if (PhysicsHelper.IsInMask(g.layer, hitLayers))
            {
                OnHit.Invoke(new PhysicalHitState()
                {
                    Other = other,
                    Position = g.transform.position,
                    Velocity = _preCollisionVelocity,
                    Direction = g.transform.forward,
                    Speed = rb.linearVelocity.magnitude
                });
            }
        }

        public virtual void Move(PhysicalActiveState state)
        {
            OnMove?.Invoke(state);
        }
    }
}