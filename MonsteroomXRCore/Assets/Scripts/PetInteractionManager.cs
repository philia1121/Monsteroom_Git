using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PetInteractionManager : MonoBehaviour
{
    [SerializeField]private bool RHandInRange;
    [SerializeField]private bool LHandInRange;
    [Header("")]
    public UnityEvent IsPettingEvent, IsNotPettingEvent;
    [SerializeField]private bool showLog = false;
    bool wasPetting; 
    public void SetRHandInRange(bool value){ RHandInRange = value;}
    public void SetLHandInRange(bool value){ LHandInRange = value;}

    void Start()
    {
        wasPetting = RHandInRange | LHandInRange;
    }

    public void CheckPettingState()
    {
        var isPetting = RHandInRange | LHandInRange;
        if(isPetting != wasPetting)
        {
            if(isPetting)
            {
                IsPettingEvent.Invoke();
                if(showLog) Debug.Log("isPetting Triggered");
            }
            else
            {
                IsNotPettingEvent.Invoke();
                if(showLog) Debug.Log("isNotPetting Triggered");
            }
        }

        wasPetting = RHandInRange | LHandInRange;
    }
}
