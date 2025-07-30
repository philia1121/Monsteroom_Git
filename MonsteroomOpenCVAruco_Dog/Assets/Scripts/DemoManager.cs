using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemoManager : MonoBehaviour
{
    public UnityEvent OnDemoStartEvent;
    public UnityEvent OnDemoEndEvent;
    [SerializeField] private bool showLog = false;

    public void OnDemoStart()
    {
        OnDemoStartEvent.Invoke();
        if (showLog) Debug.Log("On Demo Start Fired");
    }
    public void OnDemoEnd()
    {
        OnDemoEndEvent.Invoke();
        if (showLog) Debug.Log("On Demo Start Fired");
    }
}
