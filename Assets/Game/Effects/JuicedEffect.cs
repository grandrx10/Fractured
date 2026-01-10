using System;
using Game.Health;
using UnityEngine;

namespace Game.Effects
{
    public class JuicedEffect : PlayerEffect
    {
        public override bool Unique => true;
        public GameObject effect;
        private void Start()
        {
            var g = Resources.Load<GameObject>("Effects/JuicedEffect");
            effect = Instantiate(g, env.player.transform.position, Quaternion.identity, env.player.transform);
        }

        private void OnDestroy()
        {
            Destroy(effect);
        }

        public override void TickEffect(float dt)
        {
        }
    }
}