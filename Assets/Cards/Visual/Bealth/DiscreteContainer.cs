using UnityEngine;
using System.Collections.Generic;

public class DiscreteContainer : MonoBehaviour
{
    public DiscreteDisplay itemPrefab; // prefab with SetValue(float)
    public int maxPerContainer = 5;    // max value per display
    public Transform container;        // parent for the items
    public bool flip;
    private readonly List<DiscreteDisplay> items = new();
    private int currentMaxDisplays = 0;

    /// <summary>
    /// Sets the number of displays.
    /// Reinitializes prefabs if the count changed.
    /// </summary>
    public void SetMaxValue(int maxValue)
    {
        // print("something there");
        if (maxValue == currentMaxDisplays)
            return;

        currentMaxDisplays = Mathf.CeilToInt((float)maxValue / maxPerContainer);
        print($"{currentMaxDisplays}, {maxValue}");
        foreach (Transform t in container)
        {
            Destroy(t.gameObject);
        }
        items.Clear();

        // instantiate new items
        for (int i = 0; i < currentMaxDisplays; i++)
        {
            var newItem = Instantiate(itemPrefab, container);
            newItem.SetValue(0f); // initialize to zero
            items.Add(newItem);
            print("yes");
        }

        if (flip) items.Reverse();
    }

    /// <summary>
    /// Sets the total value and distributes it greedily among displays.
    /// Each display gets up to maxPerContainer.
    /// </summary>
    public void SetValue(float value)
    {
        foreach (var item in items)
        {
            if (value <= 0f)
            {
                item.SetValue(0f);
            }
            else
            {
                float assign = Mathf.Min(value, maxPerContainer);
                item.SetValue(assign);
                value -= assign;
            }
        }
    }
}