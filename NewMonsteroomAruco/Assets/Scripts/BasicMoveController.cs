using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicMoveController : MonoBehaviour
{
    CharacterController controller;
    [SerializeField]private float moveSpeed = 5;
    [SerializeField]private float rotateSpeed = 5;
    Vector3 moveDir;
    [SerializeField]private bool reverseX, reverseZ;
    ControlMap controlMap;
    void Awake()
    {
        controlMap = new ControlMap();   

        try
        {
            controller = this.GetComponent<CharacterController>();
        }
        catch
        {
            controller = this.AddComponent<CharacterController>();
        }
    }
    void OnEnable()
    {
        controlMap.PlayerInput.Enable();

        controlMap.PlayerInput.Move.started += MoveControl;
        controlMap.PlayerInput.Move.performed += MoveControl;
        controlMap.PlayerInput.Move.canceled += MoveControl;
    }
    void OnDisable()
    {
        controlMap.PlayerInput.Move.started -= MoveControl;
        controlMap.PlayerInput.Move.performed -= MoveControl;
        controlMap.PlayerInput.Move.canceled -= MoveControl;

        controlMap.PlayerInput.Disable();
    }
    public void Start()
    {
        
    }
    void Update()
    {
        controller.Move(moveDir* moveSpeed* Time.deltaTime);
        HandleRotation();
    }

    void MoveControl(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>(); 
        var reX = reverseX? -1 : 1; 
        var reZ = reverseZ? -1 : 1;
        moveDir = new Vector3(value.x* reX, 0, value.y* reZ).normalized;   
    }

    void HandleRotation()
    {
        if(moveDir.magnitude <= 0)  return;
        
        var facing = moveDir;
        facing.y = 0;

        Quaternion current = this.transform.rotation;
        Quaternion target = Quaternion.LookRotation(facing);

        if(Vector3.Distance(current.eulerAngles, target.eulerAngles) > 1f )
        {
            var step = rotateSpeed* 100* Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(current, target, step);
        }
        else
        {
            transform.rotation = target;
        }
    }
}
