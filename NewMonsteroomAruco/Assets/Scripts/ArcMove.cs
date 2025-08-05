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
    public bool turnOnStart = true;
    public float turnSpeed = 5f;
    bool isTurning = true;
    Quaternion targetRotation;
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
        BeginMove();
        enabled = true;
    }
    public void HeadBack()
    {
        t = 0;
        startPoint = pointB.position;
        endPoint = pointA.position;
        ComputeControlPoint();
        BeginMove();
        enabled = true;
    }
    void Update()
    {
        if (isTurning)
        {
            transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            Time.deltaTime * turnSpeed * 100f
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTurning = false;
            }
            return;
        }

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
    public void BeginMove()
    {
        if (!turnOnStart)
        {
            isTurning = false;
            return;
        }
        float t0 = 0f;
        float t1 = 0.01f;
        Vector3 p0 = Mathf.Pow(1 - t0, 2) * startPoint +
                    2 * (1 - t0) * t0 * controlPoint +
                    Mathf.Pow(t0, 2) * endPoint;

        Vector3 p1 = Mathf.Pow(1 - t1, 2) * startPoint +
                    2 * (1 - t1) * t1 * controlPoint +
                    Mathf.Pow(t1, 2) * endPoint;

        Vector3 moveDir = (p1 - p0).normalized;
        if (moveDir.sqrMagnitude > 0.001f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
            isTurning = true;
        }

        t = 0f;
    }

}
