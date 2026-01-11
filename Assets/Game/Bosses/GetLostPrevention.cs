using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

public class GetLostPrevention : MonoBehaviour
{
    public GameObject effectAtBefore;
    public GameObject effectAtTarget;
    public float tpDelay, tpPostDelay;
    public Transform center;

    public float maxDist;

    private bool _teleporting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_teleporting == false && Vector3.Distance(center.position, transform.position) > maxDist)
        {
            _teleporting = true;
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        if (effectAtTarget) Instantiate(effectAtTarget, center.position, Quaternion.identity);
        yield return new WaitForSeconds(tpPostDelay);
        if (effectAtBefore) Instantiate(effectAtBefore, transform.position, Quaternion.identity);
        transform.position = center.position;
        _teleporting = false;
    }

    private void OnDrawGizmos()
    {
        if (center) Gizmos.DrawWireSphere(center.position, maxDist);
    }
}
