using UnityEngine;
using UnityEngine.InputSystem;
public class ClipSwitcher : MonoBehaviour
{
    MyInputMap myInputMap;
    public Animator myAnimator;
    public int currentIndex = 0;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.NextClip.started += ctx => SwitchClip(1);
        myInputMap.TestKey.PreviousClip.started += ctx => SwitchClip(-1);
    }

    void Start()
    {

    }
    void OnDisable()
    {
        myInputMap.TestKey.Disable();
    }
    void SwitchClip(int i)
    {
        currentIndex += i;
        if (currentIndex > 112) currentIndex = 0;
        myAnimator.SetInteger("Index", currentIndex);
    }

}
