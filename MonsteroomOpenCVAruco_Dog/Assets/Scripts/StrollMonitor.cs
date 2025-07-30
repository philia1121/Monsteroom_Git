using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StrollMonitor : MonoBehaviour
{
    [SerializeField]private Transform user;
    [SerializeField]private float idleThreshold = 5;
    [SerializeField]private float minDistanceThreshold = 0.01f;
    [SerializeField]private float distanceThreshold = 3;

    [SerializeField]private UnityEvent ReachDistanceEvent;
    [SerializeField]private UnityEvent IdlingEvet;

    Vector3 orientPos, lastPos;
    float idleTimer;
    bool isIdle;

    void Start()
    {
        orientPos = user.position;
        lastPos = user.position;
    }

    void Update()
    {
        if(!user)   return;
        
        var currentPos = user.position;
        var moveDistance = Vector3.Distance(lastPos, currentPos);

        if(moveDistance > minDistanceThreshold)
        {
            idleTimer = 0;
            isIdle = false;
            lastPos = currentPos;

            if(Vector3.Distance(orientPos, currentPos) >= distanceThreshold)
            {
                orientPos = currentPos;
                ReachDistanceEvent?.Invoke();
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
            if(idleTimer >= idleThreshold && !isIdle)
            {
                isIdle = true;
                IdlingEvet?.Invoke();
            }
        }
    }

}
