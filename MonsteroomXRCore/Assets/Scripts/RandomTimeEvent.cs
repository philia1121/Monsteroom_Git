using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RandomTimeEvent : MonoBehaviour
{
    [SerializeField] private float minInterval, maxInterval;
    [SerializeField] private bool keepRandom = true;
    [SerializeField] private bool showLog = false;
    public UnityEvent RandomTriggerEvent;
    bool doRandom;
    IEnumerator cor;

    public void SetRandomTimer(bool start)
    {
        if (start)
        {
            if (cor != null) return;
            cor = RandomTimer();
            StartCoroutine(cor);
        }
        else
        {
            if (cor != null) StopCoroutine(cor);
            cor = null;
        }
    }

    IEnumerator RandomTimer()
    {
        doRandom = true;
        while (doRandom)
        {
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);
            if (doRandom) RandomTriggerEvent.Invoke();
            if (showLog) Debug.Log("Random Timer Event Invoked");
            if (!keepRandom) doRandom = false;
        }
    }

}
