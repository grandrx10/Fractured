using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Utils;
using World.Domain;

namespace Cards.Card_Assets.Tarot.Behaviors
{
    [CreateAssetMenu(fileName = "Fool", menuName = "Behaviors/TheFool")]
    public class Fool: Behavior, IBehaviorHoldListener, IBehaviorUseListener, IBehaviorTickListener, IBehaviorHasStateTag
    {
        [PrefabComponent] public Material foolMaterial;
        public LayerMask foolLayer;
        private bool _effectActive;
        private float _lastUseTime;
        
        public void StartHold()
        {
            if (_lastUseTime < Time.time)
            {
                ToggleEffect(true);
            }
        }

        public void StopHold()
        {
            ToggleEffect(false);
        }

        public virtual bool Use(CardEnv env, Agent agent)
        {
            if (env is OpenWorldEnv opEnv)
            {
                var look = PhysicsHelper.MainObj(opEnv.GetPlayerLookTarget(foolLayer).collider);
                if (look && look.TryGetComponent(out IDomainTriggerable domain))
                {
                    domain.Trigger();
                    _lastUseTime = Time.time + 10;
                    ToggleEffect(false);
                }
            }
            else
            {
                Debug.LogError("Env Does not support throwing");
            }
            return true;
        }

        private void ToggleEffect(bool active)
        {
            _effectActive = active;
            foolMaterial.SetFloat("_Intensity", active? 1 : 0);
            DomainListener.DomainEffect?.Invoke(active);
        }

        public void Tick(CardEnv env, Agent agent)
        {
            if (!_effectActive)
            {
                if (_lastUseTime > Time.time)
                {
                    var t = _lastUseTime - Time.time;
                    if (t < 5)
                    {
                        foolMaterial.SetFloat("_Intensity", 1 - t/5);
                    }
                }
                else
                {
                    ToggleEffect(true);
                }
            }
        }
    }
}