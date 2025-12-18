using UnityEngine;

namespace World.Grass
{
    public class ShaderInteractor : MonoBehaviour
    {
        public float radius = 1f;

        private void Awake()
        {
            if (GrassComputeScript.computesSet.Count == 0)
            {
                Destroy(this);
                return;
            }
            GrassComputeScript.interactorSet.Add(this);
            foreach (var c in GrassComputeScript.computesSet)
            {
                c.UpdateInteractors();
            }
        }

        private void OnDestroy()
        {
            if (GrassComputeScript.computesSet.Count == 0)
            {
                return;
            }
            GrassComputeScript.interactorSet.Remove(this);
            foreach (var c in GrassComputeScript.computesSet)
            {
                c.UpdateInteractors();
            }
        }
    }
}