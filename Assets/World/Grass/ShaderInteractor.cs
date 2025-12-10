using System;
using UnityEngine;
 
public class ShaderInteractor : MonoBehaviour
{
    public float radius = 1f;

    private void Awake()
    {
        var c = FindFirstObjectByType<GrassComputeScript>();
        if (c) c.UpdateInteractors();
        else Destroy(this);
    }

    private void OnDestroy()
    {
        var c = FindFirstObjectByType<GrassComputeScript>();
        if (c) c.UpdateInteractors();
    }
}