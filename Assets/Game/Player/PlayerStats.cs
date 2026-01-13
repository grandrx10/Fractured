using UnityEngine;

namespace Characters.Player
{
    [CreateAssetMenu(fileName = "New Player Stats", menuName = "Characters/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        public int maxMana;
        public float manaRegen;
        public float speed;
        public float jumpPower;
        public int health;
        public enum Stat
        {
            MaxMana,
            ManaRegen,
            Speed,
            JumpPower,
            Health,
        }

        public static PlayerStats Single(Stat stat, float value)
        {
            switch (stat)
            {
                case Stat.MaxMana:
                    var a = CreateInstance<PlayerStats>();
                    a.maxMana = Mathf.RoundToInt(value);
                    return a;
                case Stat.ManaRegen:
                    var b = CreateInstance<PlayerStats>();
                    b.manaRegen = value;
                    return b;
                case Stat.Speed:
                    var c = CreateInstance<PlayerStats>();
                    c.speed = value;
                    return c;
                case Stat.JumpPower:
                    var d = CreateInstance<PlayerStats>();
                    d.jumpPower = value;
                    return d;
                case Stat.Health:
                    var e = CreateInstance<PlayerStats>();
                    e.health = Mathf.RoundToInt(value);
                    return e;
                default:
                    return null;
            }
        }
        public static string ToName(Stat stat, bool caps)
        {
            string s;
            switch (stat)
            {
                case Stat.MaxMana:
                    s = "Max Mana";
                    break;
                case Stat.ManaRegen:
                    s = "Mana Regen";
                    break;
                case Stat.Speed:
                    s = "Speed";
                    break;
                case Stat.JumpPower:
                    s = "Jump Power";
                    break;
                case Stat.Health:
                    s = "Health";
                    break;
                default:
                    s = "";
                    break;
            }
            return caps ? $"{s}" : s.ToLower();
        }
        public override string ToString()
        {
            return $"Stats: {maxMana} max mana, {manaRegen:F1} mana, {speed:F1} speed, {jumpPower:F1} jump power, {health} health";
        }

        public static PlayerStats Empty => CreateInstance<PlayerStats>();
        
        public static PlayerStats operator +(PlayerStats a, PlayerStats b)
        {
            if (!a) return b;
            if (!b) return a;
            var s = CreateInstance<PlayerStats>();
            s.maxMana = a.maxMana + b.maxMana;
            s.manaRegen = a.manaRegen + b.manaRegen;
            s.speed = a.speed + b.speed;
            s.jumpPower = a.jumpPower + b.jumpPower;
            s.health = a.health + b.health;
            return s;
        }
    }
}