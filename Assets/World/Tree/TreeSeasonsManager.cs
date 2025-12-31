using System.Collections.Generic;
using UnityEngine;

public class TreeSeasonsManager : MonoBehaviour
{
    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter,
    }
    
    public Season season;
    
    [ContextMenu("swap seasons")]
    void Set()
    {
        foreach (var seasons in GetComponentsInChildren<TreeSeasons>())
        {
            seasons.Swap();
        }
    }
}
