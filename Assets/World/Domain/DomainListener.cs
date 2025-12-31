using System;
using UnityEngine;

public class DomainListener : MonoBehaviour
{
    public static Action<bool> DomainEffect;
    public static bool CurrentDomain;

    public GameObject effect;
    private void OnEnable()
    {
        DomainEffect += Do;
        effect.SetActive(CurrentDomain);
    }

    private void OnDisable()
    {
        DomainEffect -= Do;
    }

    private void Do(bool b)
    {
        CurrentDomain = b;
        effect.SetActive(b);
    }
}
