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
        ForShots();
    }

    void SetMovingingSpeed()
    {
        var ins = SpeedManager.Instance;
        var speed = (ins.Speed - ins.SpeedMin) / (ins.SpeedMax - ins.SpeedMin);
        animator.SetFloat(MovingSpeed_Hash, speed);
    }
    void ForShots()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger("Yawn");
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            animator.SetBool("IsSleeping", !animator.GetBool("IsSleeping"));
        }
    }
}
