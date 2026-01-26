using Game;
using UnityEngine;

public class LoadOnTag : MonoBehaviour
{
    public GameObject before, after;
    public string eventName;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bool b = GlobalState.instance.HasEvent(eventName);
        before.SetActive(!b);
        after.SetActive(b);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
