using System;
using UnityEngine;
using UniversalForwardPlusVolumetric;
using Utils;

public class FogChanger : MonoBehaviour
{
    public float normalLightStr, forestLightStr;

    public float changeSpeedNormal, changeSpeedForest;

    public bool isIn;
    public VolumetricConfig config;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        config.directionalScatteringIntensity = Mathf.MoveTowards(config.directionalScatteringIntensity,
            isIn ? forestLightStr : normalLightStr, (isIn ? changeSpeedForest : changeSpeedNormal)* Time.deltaTime);
    }

    private void OnDisable()
    {
        config.directionalScatteringIntensity = normalLightStr;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhysicsHelper.MainObj(other).CompareTag("Player"))
        {
            isIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhysicsHelper.MainObj(other).CompareTag("Player"))
        {
            isIn = false;
        }
    }
}
