using UnityEngine;
using UnityEngine.InputSystem;
public class ModelDisplayCalibrator : MonoBehaviour
{
    public Transform scalingTransform, offsetTransform;
    public GameObject FPS_Board;
    MyInputMap myInputMap;
    int currentAdjust = 1;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.Rescale.started += Rescale;
        myInputMap.TestKey.Rescale.performed += Rescale;
        myInputMap.TestKey.Rescale.canceled += Rescale;
        myInputMap.TestKey.Rescale.canceled += ctx => SavePrefs();
        myInputMap.TestKey.PlaceAdjust.started += PlaceOffsetAdjust;
        myInputMap.TestKey.PlaceAdjust.performed += PlaceOffsetAdjust;
        myInputMap.TestKey.PlaceAdjust.canceled += PlaceOffsetAdjust;
        myInputMap.TestKey.PlaceAdjust.canceled += ctx => SavePrefs();
        myInputMap.TestKey.NextPlaceOption.started += ctx => SelectAdjust();
        myInputMap.TestKey.DeletePrefs.started += ctx => DeletePrefs();
        myInputMap.TestKey.SettingsToggle.started += ctx => SettingsToggle();
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
    void Rescale(InputAction.CallbackContext ctx)
    {
        if (!FPS_Board.activeSelf) return;
        Vector2 value = ctx.ReadValue<Vector2>();
        if (value.y > 0)
        {
            scalingTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
        else if (value.y < 0)
        {
            scalingTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }
    }
    void SelectAdjust()
    {
        if (!FPS_Board.activeSelf) return;
        currentAdjust += 1;
        if (currentAdjust > 2) currentAdjust = 0;
    }
    void PlaceOffsetAdjust(InputAction.CallbackContext ctx)
    {
        if (!FPS_Board.activeSelf) return;
        Vector2 value = ctx.ReadValue<Vector2>();
        var nowPos = offsetTransform.position;
        var adjustment = new Vector3(currentAdjust == 0 ? value.y * 0.05f : 0, currentAdjust == 1 ? value.y * 0.05f : 0, currentAdjust == 2 ? value.y * 0.05f : 0);
        offsetTransform.position = nowPos + adjustment;
    }
    void SavePrefs()
    {
        PlayerPrefs.SetFloat("localScale", scalingTransform.localScale.x);
        PlayerPrefs.GetFloat("posX", offsetTransform.position.x);
        PlayerPrefs.SetFloat("posY", offsetTransform.position.y);
        PlayerPrefs.GetFloat("posZ", offsetTransform.position.z);
        PlayerPrefs.Save();
    }
    void GetPrefs()
    {
        var scale = PlayerPrefs.GetFloat("localScale", scalingTransform.localScale.x);
        var posX = PlayerPrefs.GetFloat("posX", offsetTransform.position.x);
        var posY = PlayerPrefs.GetFloat("posY", offsetTransform.position.y);
        var posZ = PlayerPrefs.GetFloat("posZ", offsetTransform.position.z);
        scalingTransform.localScale = new Vector3(scale, scale, scale);
        offsetTransform.position = new Vector3(posX, posY, posZ);
    }
    void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        dontSave = true;
    }
    bool dontSave = false;

    void SettingsToggle()
    {
        FPS_Board.SetActive(!FPS_Board.activeSelf);
    }
}
