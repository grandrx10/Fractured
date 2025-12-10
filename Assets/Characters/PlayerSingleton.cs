using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance { get; private set; }

    private void Awake()
    {
        // If another instance exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Register this instance
        Instance = this;

        // Optional: keep player between scene loads
        // DontDestroyOnLoad(gameObject);
    }
}
