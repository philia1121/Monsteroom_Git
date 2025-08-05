using UnityEngine;
using UnityEngine.InputSystem;
public class AdjustYPlacement : MonoBehaviour
{
    MyInputMap myInputMap;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.PlaceY.started += PlaceY;
        myInputMap.TestKey.PlaceY.performed += PlaceY;
        myInputMap.TestKey.PlaceY.canceled += PlaceY;
    }
    void PlaceY(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        var nowPos = this.transform.position;
        this.transform.position = new Vector3(nowPos.x, nowPos.y + value.y * 0.1f, nowPos.z);
    }
}
