using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : DiscreteDisplay
{
    public Image heart;
    public Sprite full, half, empty;
    public override void SetValue(float value)
    {
        if (value <= 0)
        {
            heart.sprite = empty;
        } else if (value >= 2)
        {
            heart.sprite = full;
        }
        else
        {
            heart.sprite = half;
        }
    }
}
