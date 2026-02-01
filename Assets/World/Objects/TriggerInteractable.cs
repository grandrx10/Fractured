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
        public bool repeat = true;
        public UnityEvent onInteract;

        Coroutine waitRoutine;
        private bool _ac;

        private void OnTriggerEnter(Collider other)
        {
            if (!repeat && _ac) return;
            if (!PhysicsHelper.MainObj(other).CompareTag("Player"))
                return;

            if (time <= 0f)
            {
                _ac = true;
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
            _ac = true;
            onInteract.Invoke();
            waitRoutine = null;
        }
    }
}
