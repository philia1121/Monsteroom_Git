using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteedBehaviour : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * SpeedManager.Instance.Speed * Time.deltaTime, Space.World);
    }
}
