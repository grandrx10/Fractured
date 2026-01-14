using System.Collections.Generic;
using UnityEngine;

public class TreeSeasons : MonoBehaviour
{
    public List<Material> leaf, trunk;
    
    public void Swap()
    {
        var data = GetComponentInParent<TreeSeasonsManager>();
        if (!data) Debug.LogError("No season data found");
        foreach (MeshRenderer obj in GetComponentsInChildren<MeshRenderer>())
        {
            if (obj.name.Contains("leaf"))
            {
                obj.material = leaf[(int)data.season];
            }
            else if (obj.name.Contains("tree"))
            {
                obj.material = trunk[(int)data.season];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
