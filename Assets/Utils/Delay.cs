using System;
using System.Collections;
using UnityEngine;


namespace Utils
{

    public static class Delay
    {
        private class DelayRunner : MonoBehaviour { }
        private static DelayRunner runner;

        private static void EnsureRunner()
        {
            if (runner == null)
            {
                var go = new GameObject("[GlobalDelayRunner]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                runner = go.AddComponent<DelayRunner>();
            }
        }

        // ----------------------------------------
        // GLOBAL DELAY (always runs)
        // ----------------------------------------
        public static void Call(float time, Action callback)
        {
            EnsureRunner();
            runner.StartCoroutine(RunDelay(time, callback));
        }

        // ----------------------------------------
        // OBJECT-BOUND DELAY (cancels if object destroyed)
        // ----------------------------------------
        public static void Call(MonoBehaviour owner, float time, Action callback)
        {
            if (owner == null) return; // destroyed already
            owner.StartCoroutine(RunDelayBound(owner, time, callback));
        }

        public static void Call(GameObject owner, float time, Action callback)
        {
            if (owner == null) return; // destroyed already
            var mb = owner.GetComponent<MonoBehaviour>();
            if (mb == null)
            {
                // fallback: attach a small runner
                mb = owner.AddComponent<LocalDelayMarker>();
            }
            mb.StartCoroutine(RunDelayBound(owner, time, callback));
        }

        private static IEnumerator RunDelay(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private static IEnumerator RunDelayBound(UnityEngine.Object owner, float time, Action callback)
        {
            yield return new WaitForSeconds(time);

            // If owner was destroyed → cancel automatically
            if (owner == null)
                yield break;

            callback?.Invoke();
        }

        private class LocalDelayMarker : MonoBehaviour { }
    }
}