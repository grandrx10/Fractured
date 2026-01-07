using Cards.Environments;
using UnityEngine;

namespace Game.Effects
{
    public class StairSection : MonoBehaviour
    {
        public float appearDistance = 1.2f;
        public float disappearDistance = 2.5f;
        public Renderer stair;
        public float t;
        public Animator animator;
        void Update()
        {
            float d = Vector3.Distance(OpenWorldEnv.Current.PlayerPos, transform.position);
            animator.SetBool("appear", d < appearDistance);
            animator.SetBool("disappear", d > disappearDistance);
            stair.material.SetFloat("_t", t);
        }
    }

}