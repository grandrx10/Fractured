using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class FloatBar : MonoBehaviour
    {
        public FloatBindingDefinition bindingDefinition;
        private FloatBinding binding;

        public Slider slider;
        public float maxValue = 100f;

        void Awake()
        {
            binding = FloatBindingFactory.Create(bindingDefinition);
        }

        void Update()
        {
            if (binding == null || binding.Get == null) return;
            slider.value = binding.Value / maxValue;
        }
    }
}