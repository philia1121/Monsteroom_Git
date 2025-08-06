using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeeThroughToggler : MonoBehaviour
{
    MyInputMap myInputMap;
    public GameObject environment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.EnvironmentToggle.started += ctx => EnvironmentToggle();
    }
    void OnDisable()
    {
        myInputMap.TestKey.EnvironmentToggle.started -= ctx => EnvironmentToggle();
        myInputMap.TestKey.Disable();
    }
    void EnvironmentToggle()
    {
        environment.SetActive(!environment.activeSelf);
    }
}
