using System;
using System.Collections;
using Characters.Interactables;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace World.Objects
{
    public class TriggerInteractable : MonoBehaviour
    {
        public float time;
        public UnityEvent onInteract;

        Coroutine waitRoutine;

        private void OnTriggerEnter(Collider other)
        {
            if (!PhysicsHelper.MainObj(other).CompareTag("Player"))
                return;

            if (time <= 0f)
            {
                onInteract.Invoke();
                return;
            }

            if (waitRoutine == null)
                waitRoutine = StartCoroutine(WaitAndTrigger());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!PhysicsHelper.MainObj(other).CompareTag("Player"))
                return;

            if (waitRoutine != null)
            {
                StopCoroutine(waitRoutine);
                waitRoutine = null;
            }
        }

        IEnumerator WaitAndTrigger()
        {
            yield return new WaitForSeconds(time);
            onInteract.Invoke();
            waitRoutine = null;
        }
    }
}
