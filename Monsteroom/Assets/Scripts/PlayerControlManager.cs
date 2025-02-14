using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlManager : MonoBehaviour
{
    public Transform ScenePlane;
    public Transform LH, RH;
    public float turnSensitivity = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float turnAmount = (LH.position.z - RH.position.z) * turnSensitivity;
        turnAmount = Mathf.Clamp(turnAmount, -45f, 45f);

        ScenePlane.Rotate(Vector3.up * turnAmount * Time.deltaTime);
    }
}
