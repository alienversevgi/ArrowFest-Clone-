using System;
using System.Collections;
using UnityEngine;

namespace Utility
{
    public class Timer : Singleton<Timer>
    {
        public Coroutine StartTimer(float time, Action action)
        {
            return StartCoroutine(WaitAndExecute(time, action));
        }

        public void Stop(Coroutine timer)
        {
            if (timer != null)
                StopCoroutine(timer);
        }

        public void StopAllCoroutine()
        {
            StopAllCoroutines();
        }

        private IEnumerator WaitAndExecute(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action?.Invoke();
        }
    }
}