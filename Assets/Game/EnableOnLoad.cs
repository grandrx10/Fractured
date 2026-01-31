using Game;
using UnityEngine;

public class EnableOnLoad : MonoBehaviour
{
    public string eventName;
    public bool reverse;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GlobalState.instance.HasEvent(eventName))
        {
            gameObject.SetActive(!reverse);
        }
        else
        {
            gameObject.SetActive(reverse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
