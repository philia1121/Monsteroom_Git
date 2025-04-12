using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SimpleTimeEvent : MonoBehaviour
{
    [SerializeField]private bool onStartTimer;
    public UnityEvent StartTimerEvent;
    [SerializeField]private bool onEndTimer;
    public UnityEvent EndTimerEvent;
    [SerializeField]private bool showLog = false;

    public void SetSimpleTimer(float duration)
    {
        if(onStartTimer)
            StartTimerEvent.Invoke();
            if(showLog) Debug.Log("Invoke on Start Timer: " + this.gameObject.name);
        Invoke("SimpleWait", duration);
    }
    void SimpleWait()
    {
        EndTimerEvent.Invoke();
        if(showLog) Debug.Log("Invoke on End Timer: " + this.gameObject.name);
    }
}
