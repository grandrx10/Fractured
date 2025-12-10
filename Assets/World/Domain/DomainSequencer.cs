using System;
using UnityEngine;
using World.Objects;

public class DomainSequencer : MonoBehaviour
{
    public float radius, height;
    [Range(0,1)] public float tOut, tIn;
    public Material domainMat;
    public Material shadeMat;
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool ok = URPRendererFeatureUtility.SetFeatureEnabled<FullScreenPassRendererFeature>(Camera.main, true);
    }

    public void Interact(BaseInteractable I, GameObject player)
    {
        animator.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        domainMat.SetFloat("_R", radius);
        domainMat.SetFloat("_Y", height);
        domainMat.SetVector("_Center", transform.position);
        shadeMat.SetFloat("_t", tOut);
        shadeMat.SetFloat("_t2", tIn);
    }

    public void Done()
    {
        enabled = false;
        animator.enabled = false;
    }

    private void OnDestroy()
    {
        domainMat.SetFloat("_Y", 0);
        domainMat.SetFloat("_R", 0);
    }
}
