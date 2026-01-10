using System;
using Game.Health;
using UnityEngine;

namespace Game.Effects
{
    public class SpringEffect : PlayerEffect
    {
        public override bool Unique => true;
        public GameObject effect;
        private float _lt = 15;
        private void Start()
        {
            var g = Resources.Load<GameObject>("Effects/SpringEffect");
            effect = Instantiate(g, env.player.transform.position, Quaternion.identity, env.player.transform);
        }

        private void OnDestroy()
        {
            Destroy(effect);
        }

        public override void TickEffect(float dt)
        {
            _lt -= dt;
            if (_lt <= 0) Destroy(effect);
        }
    }
}