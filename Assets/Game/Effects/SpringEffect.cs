using System;
using Characters.Player;
using Game.Health;
using UnityEngine;

namespace Game.Effects
{
    public class SpringEffect : PlayerEffect
    {
        public override EffectStackBehavior Unique => EffectStackBehavior.Replace;
        public GameObject effect;
        private float _lt = 15;
        private void Start()
        {
            var g = Resources.Load<GameObject>("Effects/SpringEffect");
            effect = Instantiate(g, env.player.transform.position, Quaternion.identity, env.player.transform);
            var s = PlayerStats.Empty;
            s.speed += 4;
            env.player.stats.tempStats += s;
            print(env.player.stats.tempStats);
            env.player.UpdateStats();
        }

        private void OnDestroy()
        {
            var s = PlayerStats.Empty;
            s.speed -= 4;
            env.player.stats.tempStats += s;
            env.player.UpdateStats();
            print(env.player.stats.tempStats);
            Destroy(effect);
        }

        public override void TickEffect(float dt)
        {
            _lt -= dt;
            if (_lt <= 0) Destroy(this);
        }
    }
}