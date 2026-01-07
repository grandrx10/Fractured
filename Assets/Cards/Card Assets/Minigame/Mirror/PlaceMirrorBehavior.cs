using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "PuzzleMirror", menuName = "Behaviors/PuzzleMirror")]
    public class PlaceMirrorBehavior: LookingAtBehavior, IBehaviorHasStateTag
    {
        private bool _placed;
        public Mirror mirrorPrefab;
        private Mirror _placedMirror;
        public override bool Use(CardEnv env, Agent agent)
        {
            if (!(env is OpenWorldEnv opEnv)) return false;
            var look = LookingAt(env);

            var go = PhysicsHelper.MainObj(look.collider);
            
            if (_placed)
            {
                if (_placedMirror) Destroy(_placedMirror.gameObject);
                _placed = false;
                return true;
            }
            
            if (go)
            {
                
                if (!_placed)
                {
                    Vector3 forward =
                        Vector3.ProjectOnPlane(opEnv.PlayerLook, Vector3.up);
                    
                    if (forward.sqrMagnitude > 0.0001f)
                    {
                        float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

                        angle = Mathf.Round(angle / mirrorPrefab.rotationStepDegrees) * mirrorPrefab.rotationStepDegrees;

                        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
                        _placedMirror = Instantiate(mirrorPrefab, look.point, rot);
                        _placed = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public override string GetDescription()
        {
            return $"<b>Mirror</b>: On Use, places a mirror. If placed, retrieves it.";
        }
    }
}