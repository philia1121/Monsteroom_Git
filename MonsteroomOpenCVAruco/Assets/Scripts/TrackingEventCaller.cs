using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TrackingEventCaller : MonoBehaviour
{
    [SerializeField] private Transform trackingTarget;
    [SerializeField] private IgnoreVector3Option moveIgnore;
    [SerializeField] private IgnoreVector3Option RotationIgnore;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float moveThresholdTime;
    float moveTimer;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotateThresholdTime;
    float rotateTimer;
    Vector3 currentPos, oldPos;
    Quaternion currentRot, oldRot;
    public bool IsMoving;
    bool wasMoing;
    public bool IsRotating;
    bool wasRotating;

    void Update()
    {
        currentPos = FilteredPos(trackingTarget.position);
        currentRot = FilteredRotaion(trackingTarget.rotation);

        CheckMovingState();
        CheckRotatingState();

        oldPos = currentPos;
        oldRot = currentRot;
    }


    void CheckMovingState()
    {
        // Debug.Log(Vector3.Distance(currentPos, oldPos));
        if (Vector3.Distance(currentPos, oldPos) > moveThreshold)
        {
            moveTimer += Time.deltaTime;
            if (moveThresholdTime >= moveTimer)
            {
                IsMoving = true;
            }
        }
        else
        {
            moveTimer = 0;
            IsMoving = false;
        }

        if (IsMoving != wasMoing)
        {
            Debug.Log("Moving State Change: " + IsMoving);
        }
        wasMoing = IsMoving;
    }
    void CheckRotatingState()
    {
        if (Vector3.Distance(currentRot.eulerAngles, oldRot.eulerAngles) > rotateThreshold)
        {

        }
        else
        {

        }
    }
    Vector3 FilteredPos(Vector3 value)
    {
        float newX = moveIgnore.ignoreX ? 0 : value.x;
        float newY = moveIgnore.ignoreY ? 0 : value.y;
        float newZ = moveIgnore.ignoreZ ? 0 : value.z;
        return new Vector3(newX, newY, newZ);
    }
    Quaternion FilteredRotaion(Quaternion value)
    {
        var rot = value.eulerAngles;
        float newX = moveIgnore.ignoreX ? 0 : rot.x;
        float newY = moveIgnore.ignoreY ? 0 : rot.y;
        float newZ = moveIgnore.ignoreZ ? 0 : rot.z;
        return Quaternion.Euler(new Vector3(newX, newY, newZ));
    }

}

[System.Serializable]
public class IgnoreVector3Option
{
    public bool ignoreX, ignoreY, ignoreZ;
}
