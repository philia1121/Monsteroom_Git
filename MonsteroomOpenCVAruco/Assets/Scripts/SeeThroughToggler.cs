using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeeThroughToggler : MonoBehaviour
{
    MyInputMap myInputMap;
    public GameObject seeThrough, environment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.SeeThroughToggle.started += ctx => SeeThroughModeToggle();
        myInputMap.TestKey.EnvironmentToggle.started += ctx => EnvironmentToggle();
    }
    void OnDisable()
    {
        myInputMap.TestKey.Disable();
    }
    void SeeThroughModeToggle()
    {
        seeThrough.SetActive(!seeThrough.activeSelf);
    }
    void EnvironmentToggle()
    {
        environment.SetActive(!environment.activeSelf);
    }
}
