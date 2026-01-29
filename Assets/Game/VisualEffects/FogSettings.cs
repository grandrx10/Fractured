using UnityEngine;

namespace Game.VisualEffects
{
    [CreateAssetMenu]
    public class FogSettings : ScriptableObject
    {
        public float range, power;
        public Color color;
        public float activeState;
        public static FogSettings Empty => CreateInstance<FogSettings>();
    }
}