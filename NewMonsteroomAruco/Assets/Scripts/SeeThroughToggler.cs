using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SeeThroughToggler : MonoBehaviour
{
    MyInputMap myInputMap;
    public GameObject environment, coords, floor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.EnvironmentToggle.started += ctx => EnvironmentToggle();
        myInputMap.TestKey.SeeThroughToggle.started += ctx => SeeCoordsToggle();
        myInputMap.TestKey.AdjustFloor.started += AdjustFloor;
        myInputMap.TestKey.AdjustFloor.performed += AdjustFloor;
        myInputMap.TestKey.AdjustFloor.canceled += AdjustFloor;
        myInputMap.TestKey.AdjustFloor.canceled += ctx => SavePrefs();
        myInputMap.TestKey.DeletePrefs.started += ctx => DeletePrefs();
    }
    void OnDisable()
    {
        myInputMap.TestKey.EnvironmentToggle.started -= ctx => EnvironmentToggle();
        myInputMap.TestKey.Disable();
    }
    void Start()
    {
        GetPrefs();
    }
    void OnApplicationPause(bool pause)
    {
        if (pause && !dontSave) SavePrefs();
    }
    void OnApplicationQuit()
    {
        if (!dontSave) SavePrefs();
    }
    void EnvironmentToggle()
    {
        environment.SetActive(!environment.activeSelf);
    }
    void SeeCoordsToggle()
    {
        coords.SetActive(!coords.activeSelf);
    }
    void AdjustFloor(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        var nowPos = floor.transform.position;
        floor.transform.position = nowPos + new Vector3(0, value.y * 0.01f, 0);

    }
    void SavePrefs()
    {
        PlayerPrefs.SetFloat("floorPosY", floor.transform.position.y);
        PlayerPrefs.Save();
    }
    void GetPrefs()
    {
        var y = PlayerPrefs.GetFloat("floorPosY", floor.transform.position.y);
        floor.transform.position = new Vector3(floor.transform.position.x, y, floor.transform.position.z);
    }
    void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        dontSave = true;
    }
    bool dontSave = false;
}
