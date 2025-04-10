using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RandomTimeEvent : MonoBehaviour
{
    [SerializeField]private float minInterval, maxInterval;
    [SerializeField]private bool keepRandom = true;
    public UnityEvent RandomTriggerEvent;
    bool doRandom;
    IEnumerator cor;

    public void StartRandomTimer()
    {
        if(cor != null) return;
        cor = RandomTimer();
        StartCoroutine(cor);
    }
    public void StopRandomTimer()
    {
        if(cor != null) StopCoroutine(cor);
    }

    IEnumerator RandomTimer()
    {
        while(doRandom)
        {
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);
            if(doRandom) RandomTriggerEvent.Invoke();
            if(!keepRandom) doRandom = false;
        }
    }
    
}
