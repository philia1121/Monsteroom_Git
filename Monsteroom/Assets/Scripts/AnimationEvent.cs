using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public string Notes;
    public int TargetNum;
    public UnityEvent AnimationEventTrigger = new UnityEvent();

    void AnimationEventNumber(int i) //物件通用型animation event function //animaiton clip event選項指的是這個function
    {
        if(TargetNum != 0 && i == TargetNum)
        {
            AnimationEventTrigger.Invoke();
        }
    }

    public void ChangeTartgetNum(int value) //可以用這個使事件不會被觸發 ex: 特定範圍外不需要觸發夾子的鏡頭效果
    {
        TargetNum = value;
    }
}
