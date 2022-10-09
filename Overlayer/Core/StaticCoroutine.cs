using System.Collections;
using UnityEngine;

namespace Overlayer.Core
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
        public static Coroutine Run(IEnumerator coroutine) => Runner.StartCoroutine(coroutine);
    }
}
