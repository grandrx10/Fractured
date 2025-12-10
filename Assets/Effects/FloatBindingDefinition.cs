using System;
using UnityEngine;
[System.Serializable]
public class FloatBindingDefinition
{
    public GameObject targetObject;
    public Component targetComponent;
    public string memberName;
    public bool setterEnabled = true;
}


public static class FloatBindingFactory
{
    public static FloatBinding Create(FloatBindingDefinition def)
    {
        if (def.targetComponent == null || string.IsNullOrEmpty(def.memberName))
            return null;

        var comp = def.targetComponent;
        var type = comp.GetType();

        // Try field
        var field = type.GetField(def.memberName);
        if (field != null)
        {
            return new FloatBinding
            {
                // Note: we're not using Bind() because we already have all info here
                Get = () => (float)field.GetValue(comp),
                Set = def.setterEnabled ? (Action<float>)(v => field.SetValue(comp, v)) : null
            };
        }

        // Try property
        var prop = type.GetProperty(def.memberName);
        if (prop != null)
        {
            return new FloatBinding
            {
                Get = () => (float)prop.GetValue(comp),
                Set = (prop.CanWrite && def.setterEnabled) 
                    ? v => prop.SetValue(comp, v) 
                    : null
            };
        }

        Debug.LogError($"FloatBinding: Member '{def.memberName}' not found on {type.Name}");
        return null;
    }
}