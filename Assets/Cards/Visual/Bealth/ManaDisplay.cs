using System;
using UnityEngine;
using UnityEngine.UI;

public class ManaDisplay : DiscreteDisplay
{
    public Image mana;

    private void Awake()
    {
        mana.material = new Material(mana.material);
    }

    public override void SetValue(float value)
    {
        mana.material.SetFloat("_t", value);
    }
}
