using UnityEngine;
using System.Collections;

public class RoutineRunner : MonoBehaviour
{
    /// <summary>
    /// Run a coroutine from a ScriptableObject or any other non-MonoBehaviour class.
    /// </summary>
    /// <param name="routine">The IEnumerator coroutine to run.</param>
    public void RunRoutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
