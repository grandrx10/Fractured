using Game;
using UnityEngine;

public class EnableOnLoad : MonoBehaviour
{
    public string eventName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GlobalState.instance.HasEvent(eventName))
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
