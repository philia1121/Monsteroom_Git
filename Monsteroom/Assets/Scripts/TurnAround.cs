using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnAround : MonoBehaviour
{
    public Transform referenceObject;
    public float turnSpeed = 5f;
    public UnityEvent DoneTurnAroundEvent;

    void Update()
    {
        if (referenceObject == null) return;

        Quaternion targetRotation = Quaternion.LookRotation(referenceObject.forward, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime * 100f
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
        {
            transform.rotation = targetRotation;
            DoneTurnAroundEvent.Invoke();
            enabled = false;
        }
    }
}
