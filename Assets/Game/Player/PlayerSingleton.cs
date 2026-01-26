using UnityEngine;

// namespace Characters
// {
//     public class PlayerSingleton : MonoBehaviour
//     {
//         public static PlayerSingleton Instance { get; private set; }
//         public LayerMask groundLayer;      // Set this to your ground layer(s)
//         public float maxRayDistance = 10f; // How far down to check for ground
//         public GameObject playerObj;
//
//         private void Awake()
//         {
//             if (Instance != null && Instance != this)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//             Instance = this;
//             transform.parent = null;
//             DontDestroyOnLoad(gameObject);
//         }
//     }
// }
