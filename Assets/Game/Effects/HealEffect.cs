using System;
using Game.Health;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Effects
{
    public class HealEffect : PlayerEffect
    {
        public override EffectStackBehavior Unique => EffectStackBehavior.Multi;
        public VisualEffect effect;
        private void Start()
        {
            var g = Resources.Load<VisualEffect>("Effects/HealEffect");
            effect = Instantiate(g, env.player.transform.position, Quaternion.identity, env.player.transform);
        }

        public void SetOrbs(int orbs)
        {
            effect.SetInt("orbs", orbs);
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