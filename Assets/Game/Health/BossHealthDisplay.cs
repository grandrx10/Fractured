using UnityEngine;
using UnityEngine.UI;
using Game.Health;

namespace Utils
{
    public class BossHealthDisplay : MonoBehaviour
    {
        [Header("Target")]
        [Tooltip("Health component to read from (boss)")]
        public Health targetHealth;

        [Header("UI")]
        public Slider slider;

        private float maxHealth;

        void Awake()
        {
            if (targetHealth == null)
            {
                Debug.LogError("BossHealthDisplay: No Health component assigned.");
                enabled = false;
                return;
            }

            maxHealth = targetHealth.health;

            if (slider != null)
            {
                slider.minValue = 0f;
                slider.maxValue = 1f;
            }
        }

        void Update()
        {
            if (targetHealth == null || slider == null)
                return;

            slider.value = targetHealth.health / maxHealth;
        }
    }
}
