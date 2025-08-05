using UnityEngine;
using UnityEngine.InputSystem;
public class ModelDisplayCalibrator : MonoBehaviour
{
    public Transform scalingTransform, posYTransform;
    MyInputMap myInputMap;
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
        myInputMap.TestKey.PlaceY.started += PlaceY;
        myInputMap.TestKey.PlaceY.performed += PlaceY;
        myInputMap.TestKey.PlaceY.canceled += PlaceY;
        myInputMap.TestKey.PlaceY.canceled += ctx => SavePrefs();
    }
    void Start()
    {
        GetPrefs();
    }
    void Rescale(InputAction.CallbackContext ctx)
    {
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
    void PlaceY(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        var nowPos = posYTransform.position;
        posYTransform.position = new Vector3(nowPos.x, nowPos.y + value.y * 0.1f, nowPos.z);
    }
    void SavePrefs()
    {
        PlayerPrefs.SetFloat("localScale", scalingTransform.localScale.x);
        PlayerPrefs.SetFloat("posY", posYTransform.position.y);
        PlayerPrefs.Save();
    }
    void GetPrefs()
    {
        var scale = PlayerPrefs.GetFloat("localScale", scalingTransform.localScale.x);
        var posY = PlayerPrefs.GetFloat("posY", posYTransform.position.y);
        scalingTransform.localScale = new Vector3(scale, scale, scale);
        posYTransform.position = new Vector3(posYTransform.position.x, posY, posYTransform.position.z);
    }
}
