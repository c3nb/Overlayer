using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overlayer.Unity
{
    public class StaticCoroutine : MonoBehaviour
    {
        static StaticCoroutine Runner
        {
            get
            {
                if (!runner)
                {
                    runner = new GameObject().AddComponent<StaticCoroutine>();
                    DontDestroyOnLoad(runner.gameObject);
                    return runner;
                }
                return runner;
            }
        }
        static StaticCoroutine runner;
        static Queue<IEnumerator> routines = new Queue<IEnumerator>();
        public static Coroutine Run(IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                _ = Runner;
                return null;
            }
            return Runner.StartCoroutine(coroutine);
        }
        public static void Queue(IEnumerator coroutine) => routines.Enqueue(coroutine);
        public static IEnumerator SyncRunner(Action routine, object firstYield = null)
        {
            yield return firstYield;
            routine?.Invoke();
            yield break;
        }
        void Update()
        {
            while (routines.Count > 0)
                StartCoroutine(routines.Dequeue());
        }
    }
}
