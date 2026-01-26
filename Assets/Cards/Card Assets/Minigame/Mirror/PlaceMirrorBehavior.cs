using Cards.Core;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using UnityEngine;
using Utils;

namespace Cards.Card_Assets.General_Behaviors
{
    [CreateAssetMenu(fileName = "PuzzleMirror", menuName = "Behaviors/PuzzleMirror")]
    public class PlaceMirrorBehavior : LookingAtBehavior, IBehaviorHasStateTag
    {
        private bool _placed;
        public Mirror mirrorPrefab;
        private Mirror _placedMirror;

        public override bool Use(Card card, CardEnv env, Agent agent)
        {
            if (!(env is OpenWorldEnv opEnv))
                return false;

            var look = LookingAt(card, env);
            var go = PhysicsHelper.MainObj(look.collider);

            // If mirror already placed → remove it
            if (_placed)
            {
                if (_placedMirror)
                    Destroy(_placedMirror.gameObject);

                _placed = false;
                _placedMirror = null;
                return true;
            }

            if (!go)
                return false;

            // Get rotation behavior from prefab (Option A)
            var rotatable = mirrorPrefab.GetComponent<DiscreteRotatable>();
            if (!rotatable)
            {
                Debug.LogError("Mirror prefab is missing DiscreteRotatable");
                return false;
            }

            Vector3 forward = Vector3.ProjectOnPlane(opEnv.PlayerLook, Vector3.up);
            if (forward.sqrMagnitude < 0.0001f)
                return false;

            float angle = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;

            float step = rotatable.rotationStepDegrees;
            angle = Mathf.Round(angle / step) * step;

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

            _placedMirror = Instantiate(mirrorPrefab, look.point, rotation);
            _placed = true;

            return true;
        }

        public override string GetDescription(Card card)
        {
            return "<b>(Active) Mirror</b>: Place a mirror. If one is already placed, retrieve it.";
        }

        public Card AttachedCard { get; set; }
    }
}
