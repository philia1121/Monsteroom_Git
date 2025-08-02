using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class TrackingEventCaller : MonoBehaviour
{
    MyInputMap myInputMap;
    [SerializeField] private Transform trackingTarget;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private IgnoreVector3Option moveIgnore;
    [SerializeField] private IgnoreVector3Option RotationIgnore;
    [SerializeField] private float timeWindow;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float moveThresholdTime;
    float moveTimer;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotateThresholdTime;
    float rotateTimer;
    [SerializeField] private float playerMoveThreshold;
    [SerializeField] private float playerMoveThresholdTime;
    float playerMoveTimer;

    [Header("UI Board")]
    public TextMesh tm;
    public UnityEngine.UI.Image move_img, rotate_img, playerMove_img, playerRotate_img;

    Vector3 currentPos, oldPos;
    Quaternion currentRot, oldRot;
    Vector3 playerCurrentPos, playerOldPos;
    Quaternion playerCurrentRot, playerOldRot;
    public bool IsMoving;
    bool wasMoving;
    public bool IsRotating;
    bool wasRotating;
    public bool IsPlayerMoving;
    bool wasPlayerMoving;
    public bool IsPlayerRotating;
    bool wasPlayerRotating;
    float minMovingDist, maxMovingDist, minRotatingDist, maxRotatingDist, movingDist, rotatingDist;
    LinkedList<(float time, Vector3 pos)> historyPos = new LinkedList<(float time, Vector3 pos)>();
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.NextClip.started += ctx => ResetValue();
    }
    void ResetValue()
    {
        minMovingDist = 0;
        maxMovingDist = 0;
        minRotatingDist = 0;
        maxRotatingDist = 0;
        ShowText();
    }

    void Update()
    {
        currentPos = FilteredPos(trackingTarget.position);
        currentRot = FilteredRotaion(trackingTarget.rotation);
        playerCurrentPos = FilteredPos(playerTransform.position);
        playerCurrentRot = FilteredRotaion(playerTransform.rotation);

        CheckMovingState();
        CheckRotatingState();
        CheckPlayerMovingState();
        CheckPlayerRotatingState();

        ShowText();
        ShowStateImage();

        oldPos = currentPos;
        oldRot = currentRot;
        playerOldPos = playerCurrentPos;
        playerOldRot = playerCurrentRot;
    }


    void CheckMovingState()
    {
        // if (Vector3.Distance(currentPos, oldPos) > moveThreshold)
        // {
        //     moveTimer += Time.deltaTime;
        //     if (moveThresholdTime >= moveTimer)
        //     {
        //         IsMoving = true;
        //     }
        // }
        // else
        // {
        //     moveTimer = 0;
        //     IsMoving = false;
        // }

        // if (IsMoving != wasMoving)
        // {
        //     Debug.Log("Moving State Change: " + IsMoving);
        // }
        // wasMoving = IsMoving;
        float now = Time.time;
        historyPos.AddLast((now, currentPos));
        while (historyPos.Count > 0 && now - historyPos.First.Value.time > timeWindow + 0.05f)
        {
            historyPos.RemoveFirst();
        }

        foreach (var record in historyPos)
        {
            float age = now - record.time;
            if (age >= timeWindow)
            {
                movingDist = Vector3.Distance(currentPos, record.pos);
            }
        }
    }
    void CheckRotatingState()
    {

    }
    void CheckPlayerMovingState()
    {

    }
    void CheckPlayerRotatingState()
    {

    }

    float DistanceBetween()
    {
        return Vector3.Distance(currentPos, playerCurrentPos);
    }
    void ShowText()
    {
        // var movingDist = Vector3.Distance(currentPos, oldPos);
        if (movingDist > 0)
        {
            if (minMovingDist == 0) minMovingDist = movingDist;
            if (movingDist < minMovingDist) minMovingDist = movingDist;
            if (movingDist > maxMovingDist) maxMovingDist = movingDist;
        }

        string t = "Pos Difference: " + movingDist + "\n"
            + "Rot Difference: " + Vector3.Distance(currentRot.eulerAngles, oldRot.eulerAngles) + "\n"
            + "Player Pos Difference: " + Vector3.Distance(currentPos, oldPos) + "\n"
            + "Player Rot Difference: " + Vector3.Distance(playerCurrentRot.eulerAngles, playerOldRot.eulerAngles) + "\n"
            + "\n"
            + "Min Moving Distance: " + minMovingDist + "\n"
            + "Max Moving Distance: " + maxMovingDist + "\n"
            + "Min Rotating Distance: " + minRotatingDist + "\n"
            + "Max Rotating Distance: " + minRotatingDist + "\n";
        tm.text = t;
    }
    void ShowStateImage()
    {
        move_img.color = Vector3.Distance(currentPos, oldPos) > 0 ? Color.green : Color.red;
        rotate_img.color = Vector3.Distance(currentRot.eulerAngles, oldRot.eulerAngles) > 0 ? Color.green : Color.red;
        playerMove_img.color = Vector3.Distance(currentPos, oldPos) > 0 ? Color.green : Color.red;
        playerRotate_img.color = Vector3.Distance(playerCurrentRot.eulerAngles, playerOldRot.eulerAngles) > 0 ? Color.green : Color.red;
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
