using UnityEngine;
using UnityEngine.Events;
public class LeashControl : MonoBehaviour
{
    public OVRInput.Controller targetController = OVRInput.Controller.RTouch;
    private Vector3 lastKnownPosition;
    private Quaternion lastKnownRotation;
    public float Swing_Threshold = 1.2f;
    public float Acceleration_Threshold = 5f;
    public bool coolDown = false;
    public float cd_time = 5f;
    public float cd_timeCount = 0;
    Vector3 lastPos_RH;
    float lastSpeed_RH;
    public UnityEvent OnSwingLeash;
    bool disableIK = false;

    void Start()
    {
        lastPos_RH = transform.position;
    }

    void Update()
    {
        // update for controller position
        if (OVRInput.GetControllerPositionTracked(targetController))
        {
            transform.position = OVRInput.GetLocalControllerPosition(targetController);
            transform.rotation = OVRInput.GetLocalControllerRotation(targetController);

            lastKnownPosition = transform.position;
            lastKnownRotation = transform.rotation;
        }
        else
        {
            transform.position = lastKnownPosition;
            transform.rotation = lastKnownRotation;
        }

        // check for swing
        float Speed_RH = (transform.position.y - lastPos_RH.y) / Time.deltaTime;
        float acceleration_RH = (Speed_RH - lastSpeed_RH) / Time.deltaTime;

        lastPos_RH = transform.position;
        lastSpeed_RH = Speed_RH;

        bool swing_RH = Speed_RH < -Swing_Threshold && acceleration_RH < -Acceleration_Threshold;

        if (!disableIK && swing_RH && !coolDown)
        {
            OnSwingLeash.Invoke();
            coolDown = true;
            cd_timeCount = 0;
        }

        if (coolDown)
        {
            cd_timeCount += Time.deltaTime;
            if (cd_timeCount > cd_time) coolDown = false;
        }
    }
    public void TriggerOnSwing()
    {
        OnSwingLeash.Invoke();
        Debug.Log("triggered");
    }
    public void SetDisabledIK(bool value) { disableIK = value; }
}
