using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
    public Transform LH, RH;

    [Header("Turning Settings")]
    public Transform ScenePlane;
    public float turnSensitivity = 2.0f;

    [Header("Acceleration Settings")]
    public float Swing_Threshold = 1.2f;
    public float Acceleration_Threshold = 5f;
    Vector3 lastPos_LH, lastPos_RH;
    float lastSpeed_LH, lastSpeed_RH;
    
    void Start()
    {
        lastPos_LH = LH.position;
        lastPos_RH = RH.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForRotation();
        CheckForAcceleration();
    }

    void CheckForRotation()
    {
        float turnAmount = (LH.position.z - RH.position.z) * turnSensitivity;
        turnAmount = Mathf.Clamp(turnAmount, -45f, 45f);

        ScenePlane.Rotate(Vector3.up * turnAmount * Time.deltaTime);
    }

    void CheckForAcceleration()
    {
        float Speed_LH = (LH.position.y - lastPos_LH.y) / Time.deltaTime;
        float Speed_RH = (RH.position.y - lastPos_RH.y) / Time.deltaTime;

        float acceleration_LH = (Speed_LH - lastSpeed_LH) / Time.deltaTime;
        float acceleration_RH = (Speed_RH - lastSpeed_RH) / Time.deltaTime;

        lastPos_LH = LH.position;
        lastPos_RH = RH.position;
        lastSpeed_LH = Speed_LH;
        lastSpeed_RH = Speed_RH;
        
        bool swing_LH = Speed_LH < -Swing_Threshold && acceleration_LH < -Acceleration_Threshold;
        bool swing_RH = Speed_RH < -Swing_Threshold && acceleration_RH < -Acceleration_Threshold;

        if(swing_LH && swing_RH)
        {
            Debug.Log("Speed Up");
            SpeedManager.Instance.SpeedUp();
        }
    }
}
