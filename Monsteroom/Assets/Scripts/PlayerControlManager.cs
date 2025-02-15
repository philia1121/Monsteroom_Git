using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
    public Transform LH, RH;

    [Header("Turning Settings")]
    public bool WorldOrientation = false;
    public Transform TurningObject;
    public float turnSensitivity = 25f;

    [Header("Acceleration Settings")]
    public float Swing_Threshold = 1.2f;
    public float Acceleration_Threshold = 5f;
    public bool RequireBothHand = true;
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
        if(WorldOrientation)
        {
            float turnAmount = (LH.position.z - RH.position.z) * turnSensitivity;
            turnAmount = Mathf.Clamp(turnAmount, -45f, 45f);

            TurningObject.Rotate(Vector3.up * turnAmount * Time.deltaTime);
        }
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

        if(RequireBothHand)
        {
            if(swing_LH && swing_RH)
            {
                SpeedManager.Instance.SpeedUp();
            }
        }
        else
        {
            if(swing_LH || swing_RH)
            {
                SpeedManager.Instance.SpeedUp();
            }
        }
    }
}
