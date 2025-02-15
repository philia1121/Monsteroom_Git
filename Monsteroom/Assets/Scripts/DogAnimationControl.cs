using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationControl : MonoBehaviour
{
    public Animator animator;
    int MovingSpeed_Hash;
    int IsMoving_Hash;
    void Awake()
    {   
        MovingSpeed_Hash = Animator.StringToHash("MovingSpeed");
        IsMoving_Hash = Animator.StringToHash("IsMoving");
    }
    void Update()
    {
        SetMovingingSpeed();
    }

    void SetMovingingSpeed()
    {
        var ins = SpeedManager.Instance;
        var speed = (ins.Speed - ins.SpeedMin) / (ins.SpeedMax - ins.SpeedMin);
        animator.SetFloat(MovingSpeed_Hash, speed);
    }
}
