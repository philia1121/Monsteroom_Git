using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using NUnit.Framework;
using OpenCvSharp.Util;
using Unity.Mathematics;
public class TrackingEventCaller : MonoBehaviour
{
    MyInputMap myInputMap;
    [SerializeField] private Transform trackingTarget;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private IgnoreVector3Option moveIgnore;
    [SerializeField] private IgnoreVector3Option rotationIgnore;
    [SerializeField] private float timeWindow;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float playerTimeWindow;
    [SerializeField] private float playerMoveThreshold;
    [SerializeField] private float playerRotateThreshold;

    [Header("Interaction")]
    [SerializeField] private bool ignoreTrackingControl = false;

    [Header("UI Board")]
    public GameObject UI_Board;
    public TextMeshProUGUI timeWindow_text, moveThreshold_text, rotateThreshold_text;
    public TextMeshProUGUI playerTimeWindow_text, playerMoveThreshold_text, playerRotateThreshold_text;
    public TextMeshProUGUI log_text, playerLog_text;
    public UnityEngine.UI.Image move_img, rotate_img;
    public UnityEngine.UI.Image playerMove_img, playerRotate_img;
    public TextMeshProUGUI distBeween_text;

    [Header("Event")]
    public UnityEvent OnMove;
    public UnityEvent OnStopMove, OnRotate, OnStopRotate;
    public UnityEvent OnPlayerMove, OnPlayerStopMove, OnPlayerRotate, OnPlayerStopRotate;

    Vector3 currentPos, playerCurrentPos;
    Quaternion currentRot, playerCurrentRot;
    public bool IsMoving;
    bool wasMoving;
    public bool IsRotating;
    bool wasRotating;
    public bool IsPlayerMoving;
    bool wasPlayerMoving;
    public bool IsPlayerRotating;
    bool wasPlayerRotating;
    float minMovingDist, maxMovingDist, minRotatingDist, maxRotatingDist, movingDist, rotatingDist;
    float minPlayerMovingDist, maxPlayerMovingDist, minPlayerRotatingDist, maxPlayerRotatingDist, playerMovingDist, playerRotatingDist;
    LinkedList<(float time, Vector3 pos, Quaternion rot)> historyPoints = new LinkedList<(float time, Vector3 pos, Quaternion rot)>();
    LinkedList<(float time, Vector3 pos, Quaternion rot)> playerHistoryPoints = new LinkedList<(float time, Vector3 pos, Quaternion rot)>();
    LinkedList<(float time, Vector3 pos, Quaternion rot, Vector3 playerPos, Quaternion platerRot)> allHistoryPoints = new LinkedList<(float time, Vector3 pos, Quaternion rot, Vector3 playerPos, Quaternion platerRot)>();
    int adjustingVariable = 0;
    float adjustScale = 0.001f;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.ResetRecord.started += ctx => ResetObserveValue();
        myInputMap.TestKey.NextVariable.started += ctx => SelectAdjustingVariable();
        myInputMap.TestKey.Adjust.started += AdjustVariable;
        myInputMap.TestKey.Adjust.performed += AdjustVariable;
        myInputMap.TestKey.Adjust.canceled += AdjustVariable;
        myInputMap.TestKey.NextScaleAdjust.started += ctx => SelectAdjustScale();
        myInputMap.TestKey.BoardToggle.started += ctx => BoardToggler();
        myInputMap.TestKey.DeletePrefs.started += ctx => DeletePrefs();
    }
    void Start()
    {
        GetPlayerPrefs();
    }
    void OnApplicationPause(bool pause)
    {
        if (pause && !dontSave) SavePlayerPrefs();
    }
    void OnApplicationQuit()
    {
        if (!dontSave) SavePlayerPrefs();
    }

    void Update()
    {
        currentPos = IgnoreVector3Option.FilteredPosition(moveIgnore, trackingTarget.position);
        currentRot = IgnoreVector3Option.FilteredRotation(rotationIgnore, trackingTarget.rotation);
        playerCurrentPos = IgnoreVector3Option.FilteredPosition(moveIgnore, playerTransform.position);
        playerCurrentRot = IgnoreVector3Option.FilteredRotation(rotationIgnore, playerTransform.rotation);

        float now = Time.time;
        WriteHistory(historyPoints, now, currentPos, currentRot, timeWindow);
        WriteHistory(playerHistoryPoints, now, playerCurrentPos, playerCurrentRot, timeWindow);
        WriteAllHistoryList(now, currentPos, currentRot, playerCurrentPos, playerCurrentRot, timeWindow);

        CheckMovingState();
        CheckRotatingState();
        CheckPlayerMovingState();
        CheckPlayerRotatingState();

        UpdateLogBoard();
    }

    void CheckMovingState()
    {
        foreach (var record in allHistoryPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                movingDist = Vector3.Distance(currentPos, record.pos) - Vector3.Distance(playerCurrentPos, record.playerPos);
            }
        }

        if (movingDist > 0)
        {
            if (minMovingDist == 0) minMovingDist = movingDist;
            if (movingDist < minMovingDist) minMovingDist = movingDist;
            if (movingDist > maxMovingDist) maxMovingDist = movingDist;
        }

        IsMoving = (movingDist > moveThreshold) ? true : false;

        if (IsMoving != wasMoving)
        {
            if (!ignoreTrackingControl)
            {
                if (IsMoving) OnMove.Invoke();
                else OnStopMove.Invoke();
            }

        }
        wasMoving = IsMoving;
    }
    void CheckRotatingState()
    {
        foreach (var record in historyPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                rotatingDist = Quaternion.Angle(currentRot, record.rot);
            }
        }

        if (rotatingDist > 0)
        {
            if (minRotatingDist == 0) minRotatingDist = rotatingDist;
            if (rotatingDist < minRotatingDist) minRotatingDist = rotatingDist;
            if (rotatingDist > maxRotatingDist) maxRotatingDist = rotatingDist;
        }

        IsRotating = (rotatingDist > rotateThreshold) ? true : false;

        if (IsRotating != wasRotating)
        {
            if (IsRotating) OnRotate.Invoke();
            else OnStopMove.Invoke();
        }
        wasRotating = IsRotating;
    }
    void CheckPlayerMovingState()
    {
        foreach (var record in playerHistoryPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                playerMovingDist = Vector3.Distance(playerCurrentPos, record.pos);
            }
        }

        if (playerMovingDist > 0)
        {
            if (minPlayerMovingDist == 0) minPlayerMovingDist = playerMovingDist;
            if (playerMovingDist < minPlayerMovingDist) minPlayerMovingDist = playerMovingDist;
            if (playerMovingDist > maxPlayerMovingDist) maxPlayerMovingDist = playerMovingDist;
        }

        IsPlayerMoving = (playerMovingDist > playerMoveThreshold) ? true : false;

        if (IsPlayerMoving != wasPlayerMoving)
        {
            if (IsPlayerMoving) OnPlayerMove.Invoke();
            else OnPlayerStopMove.Invoke();
        }
        wasPlayerMoving = IsPlayerMoving;
    }
    void CheckPlayerRotatingState()
    {
        foreach (var record in playerHistoryPoints)
        {
            float age = Time.time - record.time;
            if (age >= timeWindow)
            {
                playerRotatingDist = Quaternion.Angle(playerCurrentRot, record.rot);
            }
        }

        if (playerRotatingDist > 0)
        {
            if (minPlayerRotatingDist == 0) minPlayerRotatingDist = playerRotatingDist;
            if (playerRotatingDist < minPlayerRotatingDist) minPlayerRotatingDist = playerRotatingDist;
            if (playerRotatingDist > maxPlayerRotatingDist) maxPlayerRotatingDist = playerRotatingDist;
        }

        IsPlayerRotating = (playerRotatingDist > playerRotateThreshold) ? true : false;

        if (IsPlayerRotating != wasPlayerRotating)
        {
            if (IsPlayerRotating) OnPlayerRotate.Invoke();
            else OnPlayerStopRotate.Invoke();
        }
    }

    float DistanceBetween()
    {
        return Vector3.Distance(currentPos, playerCurrentPos);
    }
    void UpdateLogBoard()
    {
        timeWindow_text.text = timeWindow.ToString("0.000");
        moveThreshold_text.text = moveThreshold.ToString("0.000");
        rotateThreshold_text.text = rotateThreshold.ToString("0.000");

        playerTimeWindow_text.text = timeWindow.ToString("0.000");
        playerMoveThreshold_text.text = playerMoveThreshold.ToString("0.000");
        playerRotateThreshold_text.text = playerRotateThreshold.ToString("0.000");

        move_img.color = IsMoving ? Color.green : Color.red;
        rotate_img.color = IsRotating ? Color.green : Color.red;
        playerMove_img.color = IsPlayerMoving ? Color.green : Color.red;
        playerRotate_img.color = IsPlayerRotating ? Color.green : Color.red;

        distBeween_text.text = DistanceBetween().ToString("0.000");

        string t = "Pos Difference: " + movingDist.ToString("0.0000") + "\n"
            + "Rot Difference: " + rotatingDist.ToString("0.0000") + "\n"
            + "\n"
            + "Min Moving Distance: " + minMovingDist.ToString("0.0000") + "\n"
            + "Max Moving Distance: " + maxMovingDist.ToString("0.0000") + "\n"
            + "Min Rotating Distance: " + minRotatingDist.ToString("0.0000") + "\n"
            + "Max Rotating Distance: " + maxRotatingDist.ToString("0.0000") + "\n";
        log_text.text = t;

        string pt = "Pos Difference: " + playerMovingDist.ToString("0.0000") + "\n"
            + "Rot Difference: " + playerRotatingDist.ToString("0.0000") + "\n"
            + "\n"
            + "Min Moving Distance: " + minPlayerMovingDist.ToString("0.0000") + "\n"
            + "Max Moving Distance: " + maxPlayerMovingDist.ToString("0.0000") + "\n"
            + "Min Rotating Distance: " + minPlayerRotatingDist.ToString("0.0000") + "\n"
            + "Max Rotating Distance: " + maxPlayerRotatingDist.ToString("0.0000") + "\n";
        playerLog_text.text = pt;
    }
    void BoardToggler()
    {
        UI_Board.SetActive(!UI_Board.activeSelf);
    }
    void SelectAdjustingVariable()
    {
        if (!UI_Board.activeSelf) return;

        adjustingVariable += 1;
        if (adjustingVariable > 6) adjustingVariable = 0;

        SetFontSize(timeWindow_text, 36);
        SetFontSize(moveThreshold_text, 36);
        SetFontSize(rotateThreshold_text, 36);
        SetFontSize(playerTimeWindow_text, 36);
        SetFontSize(playerMoveThreshold_text, 36);
        SetFontSize(playerRotateThreshold_text, 36);
        switch (adjustingVariable)
        {
            case 1:
                SetFontSize(timeWindow_text, 42);
                break;
            case 2:
                SetFontSize(moveThreshold_text, 42);
                break;
            case 3:
                SetFontSize(rotateThreshold_text, 42);
                break;
            case 4:
                SetFontSize(playerTimeWindow_text, 42);
                break;
            case 5:
                SetFontSize(playerMoveThreshold_text, 42);
                break;
            case 6:
                SetFontSize(playerRotateThreshold_text, 42);
                break;
            default:
                break;
        }
    }
    void SetFontSize(TextMeshProUGUI t, float value) { t.fontSize = value; }
    void SelectAdjustScale()
    {
        if (!UI_Board.activeSelf) return;

        adjustScale *= 10;
        if (adjustingVariable > 0 && adjustScale > 1) adjustScale = 0.001f;
    }
    void AdjustVariable(InputAction.CallbackContext ctx)
    {
        if (!UI_Board.activeSelf) return;

        Vector2 value = ctx.ReadValue<Vector2>();
        switch (adjustingVariable)
        {
            case 1:
                timeWindow += AdjustValue(value.y > 0);
                break;
            case 2:
                moveThreshold += AdjustValue(value.y > 0);
                break;
            case 3:
                rotateThreshold += AdjustValue(value.y > 0);
                break;
            case 4:
                timeWindow += AdjustValue(value.y > 0);
                break;
            case 5:
                playerMoveThreshold += AdjustValue(value.y > 0);
                break;
            case 6:
                playerRotateThreshold += AdjustValue(value.y > 0);
                break;
            default:
                break;
        }
    }
    float AdjustValue(bool condi) { return condi ? adjustScale : -adjustScale; }
    void ResetObserveValue()
    {
        minMovingDist = 0;
        maxMovingDist = 0;
        minRotatingDist = 0;
        maxRotatingDist = 0;
        minPlayerMovingDist = 0;
        maxPlayerMovingDist = 0;
        minPlayerRotatingDist = 0;
        maxPlayerRotatingDist = 0;
        UpdateLogBoard();
    }
    void WriteHistory(LinkedList<(float time, Vector3 pos, Quaternion rot)> list, float time, Vector3 position, Quaternion rotation, float window)
    {
        list.AddLast((time, position, rotation));
        while (list.Count > 0 && time - list.First.Value.time > window + 0.05f)
        {
            list.RemoveFirst();
        }
    }
    void WriteAllHistoryList(float time, Vector3 pos, Quaternion rot, Vector3 playerPos, Quaternion playerRot, float window)
    {
        allHistoryPoints.AddLast((time, pos, rot, playerPos, playerRot));
        while (allHistoryPoints.Count > 0 && time - allHistoryPoints.First.Value.time > window + 0.05f)
        {
            allHistoryPoints.RemoveFirst();
        }
    }
    void SavePlayerPrefs()
    {
        SetPrefsFloat("timeWindow", timeWindow);
        SetPrefsFloat("moveThreshold", moveThreshold);
        SetPrefsFloat("rotateThreshold", rotateThreshold);
        SetPrefsFloat("playerTimeWindow", timeWindow);
        SetPrefsFloat("playerMoveThreshold", playerMoveThreshold);
        SetPrefsFloat("playerRotateThreshold", playerRotateThreshold);
        PlayerPrefs.Save();
    }
    void GetPlayerPrefs()
    {
        timeWindow = PlayerPrefs.GetFloat("timeWindow", timeWindow);
        moveThreshold = PlayerPrefs.GetFloat("moveThreshold", moveThreshold);
        rotateThreshold = PlayerPrefs.GetFloat("rotateThreshold", rotateThreshold);
        // playerTimeWindow = PlayerPrefs.GetFloat("playerTimeWindow", playerTimeWindow);
        playerMoveThreshold = PlayerPrefs.GetFloat("playerMoveThreshold", playerMoveThreshold);
        playerRotateThreshold = PlayerPrefs.GetFloat("playerRotateThreshold", playerRotateThreshold);
        UpdateLogBoard();
    }
    void SetPrefsFloat(string keyName, float value) { PlayerPrefs.SetFloat(keyName, value); }
    void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        dontSave = true;
    }
    bool dontSave = false;
    public void SetIgnoreTrackingControl(bool value) { ignoreTrackingControl = value; }
}
