using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PetFood : MonoBehaviour
{
    [SerializeField]private HandGrabInteractable grabInteractable;
    [SerializeField]private Transform Parent_FoodPot, Parent_Origin;
    Vector3 defaultPos;
    Quaternion defaultRot;
    [Header("")]
    public UnityEvent OnGrabHover, OnGrabStart, OnGrabRelease;
    Oculus.Interaction.InteractableState lastState;
    
    void Start()
    {
        if(!grabInteractable) grabInteractable = GetComponent<HandGrabInteractable>();
        lastState = grabInteractable.State;

        transform.GetLocalPositionAndRotation(out defaultPos, out defaultRot);
        PutInPetFood();
    }

    public void PutInPetFood()
    {
        this.transform.parent = Parent_FoodPot;
        this.transform.SetLocalPositionAndRotation(defaultPos, defaultRot);
    }
    public void TakeOutPetFood()
    {
        this.transform.parent = Parent_Origin;
    }

    void Update()
    {
        var currentState = grabInteractable.State;
        if(currentState == lastState) return;
        
        switch(currentState)
        {
            case Oculus.Interaction.InteractableState.Normal:
                OnGrabRelease.Invoke();
                break;
            case Oculus.Interaction.InteractableState.Hover:
                OnGrabHover.Invoke();
                break;
            case Oculus.Interaction.InteractableState.Select:
                OnGrabStart.Invoke();
                break;
            case Oculus.Interaction.InteractableState.Disabled:
                OnGrabRelease.Invoke();
                break;
            default:
                break;
        }
        lastState = currentState;
    }
}
