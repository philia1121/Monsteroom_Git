using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class TrackingEventCaller : MonoBehaviour
{
    MyInputMap myInputMap;
    [SerializeField] private Transform trackingTarget;
    // [SerializeField] private Transform playerTransform;
    [SerializeField] private IgnoreVector3Option moveIgnore;
    [SerializeField] private IgnoreVector3Option RotationIgnore;
    [SerializeField] private float timeWindow;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float moveThresholdTime;
    float moveTimer;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotateThresholdTime;
    float rotateTimer;
    // [SerializeField] private float playerMoveThreshold;
    // [SerializeField] private float playerMoveThresholdTime;
    // float playerMoveTimer;

    [Header("UI Board")]
    public TextMeshProUGUI timeWindow_text;
    public TextMeshProUGUI moveThreshold_text, rotateTHreshold_text;
    public TextMeshProUGUI log_text;
    public UnityEngine.UI.Image move_img, rotate_img;



    Vector3 currentPos, oldPos;
    Quaternion currentRot, oldRot;
    // Vector3 playerCurrentPos, playerOldPos;
    // Quaternion playerCurrentRot, playerOldRot;
    public bool IsMoving;
    bool wasMoving;
    public bool IsRotating;
    bool wasRotating;
    // public bool IsPlayerMoving;
    // bool wasPlayerMoving;
    // public bool IsPlayerRotating;
    // bool wasPlayerRotating;
    float minMovingDist, maxMovingDist, minRotatingDist, maxRotatingDist, movingDist, rotatingDist;
    LinkedList<(float time, Vector3 pos, Vector3 rot)> historyPoints = new LinkedList<(float time, Vector3 pos, Vector3 rot)>();
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
        // playerCurrentPos = FilteredPos(playerTransform.position);
        // playerCurrentRot = FilteredRotaion(playerTransform.rotation);

        float now = Time.time;
        historyPoints.AddLast((now, currentPos, currentRot.eulerAngles));
        while (historyPoints.Count > 0 && now - historyPoints.First.Value.time > timeWindow + 0.05f)
        {
            historyPoints.RemoveFirst();
        }

        CheckMovingState();
        CheckRotatingState();
        CheckPlayerMovingState();
        CheckPlayerRotatingState();

        UpdateLogBoard();

        oldPos = currentPos;
        oldRot = currentRot;
        // playerOldPos = playerCurrentPos;
        // playerOldRot = playerCurrentRot;
    }

    void CheckMovingState()
    {
        #region 
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
        #endregion

        foreach (var record in historyPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                movingDist = Vector3.Distance(currentPos, record.pos);
            }
        }

        if (movingDist > 0)
        {
            if (minMovingDist == 0) minMovingDist = movingDist;
            if (movingDist < minMovingDist) minMovingDist = movingDist;
            if (movingDist > maxMovingDist) maxMovingDist = movingDist;
        }

        IsMoving = (movingDist > moveThreshold) ? true : false;
    }
    void CheckRotatingState()
    {
        foreach (var record in historyPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                rotatingDist = Vector3.Distance(currentRot.eulerAngles, record.rot);
            }
        }

        if (rotatingDist > 0)
        {
            if (minRotatingDist == 0) minRotatingDist = rotatingDist;
            if (rotatingDist < minRotatingDist) minRotatingDist = rotatingDist;
            if (rotatingDist > maxRotatingDist) maxRotatingDist = rotatingDist;
        }

        IsRotating = (rotatingDist > rotateThreshold) ? true : false;
    }
    void CheckPlayerMovingState()
    {

    }
    void CheckPlayerRotatingState()
    {

    }

    // float DistanceBetween()
    // {
    //     return Vector3.Distance(currentPos, playerCurrentPos);
    // }
    void ShowText()
    {
        // var movingDist = Vector3.Distance(currentPos, oldPos);


        string t = "Pos Difference: " + movingDist + "\n"
            + "Rot Difference: " + Vector3.Distance(currentRot.eulerAngles, oldRot.eulerAngles) + "\n"
            + "\n"
            + "Min Moving Distance: " + minMovingDist + "\n"
            + "Max Moving Distance: " + maxMovingDist + "\n"
            + "Min Rotating Distance: " + minRotatingDist + "\n"
            + "Max Rotating Distance: " + maxRotatingDist + "\n";
        log_text.text = t;
    }
    void UpdateLogBoard()
    {
        timeWindow_text.text = timeWindow.ToString();
        moveThreshold_text.text = moveThreshold.ToString();
        rotateTHreshold_text.text = rotateThreshold.ToString();

        move_img.color = IsMoving ? Color.green : Color.red;
        rotate_img.color = IsRotating ? Color.green : Color.red;

        string t = "Pos Difference: " + movingDist.ToString("0.000000") + "\n"
            + "Rot Difference: " + rotatingDist.ToString("0.000000") + "\n"
            + "\n"
            + "Min Moving Distance: " + minMovingDist.ToString("0.000000") + "\n"
            + "Max Moving Distance: " + maxMovingDist.ToString("0.000000") + "\n"
            + "Min Rotating Distance: " + minRotatingDist.ToString("0.000000") + "\n"
            + "Max Rotating Distance: " + maxRotatingDist.ToString("0.000000") + "\n";
        log_text.text = t;
    }

    public void Set_TimeWindow(float value) { timeWindow = value; }
    public void Set_TimeWindow(string value) { timeWindow = float.Parse(value); }
    public void Set_MoveThreshold(float value) { moveThreshold = value; }
    public void Set_MoveThreshold(string value) { moveThreshold = float.Parse(value); }
    public void Set_RotateThreshold(float value) { rotateThreshold = value; }
    public void Set_RotateThreshold(string value) { rotateThreshold = float.Parse(value); }
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
