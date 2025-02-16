using System.Collections;
using System.Collections.Generic;
using IdyllicFantasyNature;
using UnityEngine;

public class SteedBehaviour : MonoBehaviour
{
    public CharacterController controller;
    public Transform RH;
    // Update is called once per frame
    void Update()
    {
        Vector3 rawDirection = RH.forward;
        Vector3 moveDirection = Vector3.ProjectOnPlane(rawDirection, Vector3.up).normalized;

        var movement = moveDirection + Vector3.down*9.8f;
        controller.Move(movement* SpeedManager.Instance.Speed* Time.deltaTime);

        if (moveDirection.sqrMagnitude > 0.01f) // 確保方向有效
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
        }
    }
}
