using UnityEngine;
using UnityEngine.UI;

public class BealthDisplay : DiscreteDisplay
{
    public Image heart;
    public Sprite full, empty;
    public override void SetValue(float value)
    {
        if (value <= 0)
        {
            heart.sprite = empty;
        }
        else
        {
            heart.sprite = full;
        }
    }
}
