using UnityEngine;

namespace Characters.Player
{
    [CreateAssetMenu(fileName = "New Player Stats", menuName = "Characters/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        public float maxMana;
        public float manaRegen;
        public float speed;

        public static PlayerStats Empty => CreateInstance<PlayerStats>();
        
        public static PlayerStats operator +(PlayerStats a, PlayerStats b)
        {
            var s = CreateInstance<PlayerStats>();
            s.maxMana = a.maxMana + b.maxMana;
            s.manaRegen = a.manaRegen + b.manaRegen;
            s.speed = a.speed + b.speed;
            return s;
        }
    }
}