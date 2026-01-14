using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : DiscreteDisplay
{
    public Image heart;
    public Sprite full, half, empty;
    public bool disappearOnEmpty = false;
    public override void SetValue(float value)
    {
        if (disappearOnEmpty) gameObject.SetActive(true);
        if (value <= 0)
        {
            if (disappearOnEmpty) gameObject.SetActive(false);
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
