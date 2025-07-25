using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogStatusData : MonoBehaviour
{
    public static DogStatusData Instance { get; private set; }
    public bool IsMoving = false;
    public Transform RobotRefers;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void SetMovingState(bool value)
    {
        IsMoving = value;
    }
}
