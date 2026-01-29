using System;
using Game.Health;
using UnityEngine;
using UnityEngine.VFX;

namespace Game.Effects
{
    public class HealEffect : PlayerEffect
    {
        public override EffectStackBehavior Unique => EffectStackBehavior.UniqueId;
        public VisualEffect effect;
        public int orbs;
        private void Start()
        {
            var g = Resources.Load<VisualEffect>("Effects/HealEffect");
            effect = Instantiate(g, env.player.transform.position, Quaternion.identity, env.player.transform);
            SetOrbs(orbs);
        }

        public void SetOrbs(int o)
        {
            orbs = o;
            effect.SetInt("orbs", orbs);
            effect.Play();
        }

        private void OnDestroy()
        {
            Destroy(effect.gameObject);
        }

        public override void TickEffect(float dt)
        {
        }
    }
}