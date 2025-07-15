using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArcMove : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float curveOffset = 10f;
    public float moveSpeed = 2f;
    public UnityEvent OnReachAEvent;
    public UnityEvent OnReachBEvent;

    private Vector3 controlPoint, startPoint, endPoint;
    private float t = 0f;
    public void StartMoving()
    {
        t = 0;
        startPoint = pointA.position;
        endPoint = pointB.position;
        ComputeControlPoint();
        enabled = true;
    }
    public void HeadBack()
    {
        t = 0;
        startPoint = pointB.position;
        endPoint = pointA.position;
        ComputeControlPoint();
        enabled = true;
    }
    void Update()
    {
        if (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            MoveAlongCurve(startPoint, controlPoint, endPoint, t);
        }
        else
        {
            enabled = false;
            if (endPoint == pointA.position)
                OnReachAEvent.Invoke();
            else
                OnReachBEvent.Invoke();
        }
    }
    void MoveAlongCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        t = Mathf.Clamp01(t);
        Vector3 pos = Mathf.Pow(1 - t, 2) * start +
                      2 * (1 - t) * t * control +
                      Mathf.Pow(t, 2) * end;

        float futureT = Mathf.Clamp01(t + 0.01f);
        Vector3 futurePos = Mathf.Pow(1 - futureT, 2) * start +
                        2 * (1 - futureT) * futureT * control +
                        Mathf.Pow(futureT, 2) * end;

        Vector3 moveDir = (futurePos - pos).normalized;
        if (moveDir.sqrMagnitude > 0.001f)
            transform.forward = moveDir;
        
        transform.position = pos;
    }
    void ComputeControlPoint()
    {
        Vector3 mid = (pointA.position + pointB.position) / 2;

        Vector3 dir = pointB.position - pointA.position;
        Vector3 up = Vector3.up; 
        Vector3 perp = Vector3.Cross(dir.normalized, up).normalized;

        controlPoint = mid + perp * curveOffset;
    }
}
