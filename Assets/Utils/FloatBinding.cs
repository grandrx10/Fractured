using System;

namespace Utils
{
    public class FloatBinding
    {
        public Func<float> Get;
        public Action<float> Set;
        public float Value
        {
            get => Get != null ? Get() : 0f;
            set => Set?.Invoke(value);
        }
    }
}