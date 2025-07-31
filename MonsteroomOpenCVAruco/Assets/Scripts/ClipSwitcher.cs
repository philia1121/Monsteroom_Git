using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ClipSwitcher : MonoBehaviour
{
    public Transform myTransform;
    MyInputMap myInputMap;
    public Animator myAnimator;
    public int currentIndex = 0;
    public string currentState;
    public int[] section;
    public AnimationClip[] allClips;
    public TextMesh tm;
    IEnumerator cor;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.TestKey.Enable();
        myInputMap.TestKey.NextClip.started += ctx => SwitchClip(1);
        myInputMap.TestKey.PreviousClip.started += ctx => SwitchClip(-1);
        myInputMap.TestKey.Rescale.started += Rescale;
        myInputMap.TestKey.Rescale.performed += Rescale;
        myInputMap.TestKey.Rescale.canceled += Rescale;
    }

    void Start()
    {
        cor = NameCheck();
        StartCoroutine(cor);
    }
    void OnDisable()
    {
        myInputMap.TestKey.Disable();
    }
    void SwitchClip(int i)
    {
        currentIndex += i;
        if (currentIndex > 113) currentIndex = 0;
        if (currentIndex < 0) currentIndex = 113;
        myAnimator.SetInteger("Index", currentIndex);
        ShowText();

    }
    void SwitchSection()
    {
        for (int i = 0; i < section.Length; i++)
        {
            if (currentIndex > section[i] & currentIndex < section[i + 1])
            {
                currentIndex = section[i + 1];
            }
        }
    }
    void Rescale(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        if (value.y > 0)
        {
            myTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            ShowText();
        }
        else if (value.y < 0)
        {
            myTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            ShowText();
        }
    }
    string GetAnimatorStateName(AnimatorStateInfo info)
    {
        foreach (var clip in allClips)
        {
            if (info.IsName(clip.name))
            {
                return clip.name;
            }
        }
        return "?";
    }
    IEnumerator NameCheck()
    {
        while (true)
        {
            GetAnimatorStateName(myAnimator.GetCurrentAnimatorStateInfo(0));
            currentState = GetAnimatorStateName(myAnimator.GetCurrentAnimatorStateInfo(0));
            ShowText();
            yield return new WaitForSeconds(0.5f);
        }
    }
    void ShowText()
    {
        tm.text = "Scale: " + myTransform.localScale.x + ",  Current Animation Clip: " + currentState + ", Clip Count: " + currentIndex;
    }

}
