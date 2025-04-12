using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimatorManager : MonoBehaviour
{
    [SerializeField]Animator myAnimator;
    public void SetAnimatorBool_True(string parameter){ myAnimator.SetBool(parameter, true);}
    public void SetAnimatorBool_False(string parameter){ myAnimator.SetBool(parameter, false);}
    
    void Awake()
    {
        if(myAnimator == null) myAnimator = this.GetComponent<Animator>();   
    }
}
