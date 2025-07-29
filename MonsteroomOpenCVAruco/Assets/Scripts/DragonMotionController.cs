using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class DragonMotionController : MonoBehaviour
{
    [Header("Motion Target")]
    [SerializeField] private Transform motionTarget;
    [SerializeField] private bool targetRotReverse;

    [Header("Motion Control Settings")]
    [SerializeField] private float thresholdAngle = 8;
    [SerializeField] private float holdTime = 0.8f;

    [Header("Alignment")]
    [SerializeField] private Transform moveSubject;
    [SerializeField] private Transform rotateSubject;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float movingAniSpeed = 0.35f;

    [Header("Move and Rotate Settings")]
    [SerializeField] private float maxMove = 20;
    [SerializeField] private bool moveReverse = false;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float maxRotation = 30;
    [SerializeField] private bool rotateReverse = false;
    [SerializeField] private float rotateSpeed = 45;


    private float minY, maxY, minRotX, maxRotX;
    private float upTimer = 0f;
    private float downTimer = 0f;
    private bool isMovingUp = false;
    private bool isMovingDown = false;
    private float normalAniSpeed;

    void Start()
    {
        minY = moveSubject.position.y - maxMove;
        maxY = moveSubject.position.y + maxMove;
        minRotX = rotateSubject.rotation.x;
        maxRotX = rotateSubject.rotation.x;
        minRotX += rotateReverse ? maxRotation : -maxRotation;
        maxRotX += rotateReverse ? -maxRotation : maxRotation;
        // normalAniSpeed = animator.GetFloat("BounceSpeed");
    }

    void Update()
    {
        CheckTargetMotion();

        Vector3 rot = rotateSubject.localEulerAngles;
        Vector3 pos = moveSubject.position;

        float currentXRot = NormalizeAngle(rot.x);

        if (isMovingUp && pos.y < maxY)
        {
            currentXRot = Mathf.MoveTowards(currentXRot, maxRotX, rotateSpeed * Time.deltaTime);
            pos.y = Mathf.MoveTowards(pos.y, maxY, moveSpeed * Time.deltaTime);
            DragonStatusData.Instance.FlyDirection = 1;
            // animator.SetFloat("BounceSpeed", movingAniSpeed);
        }
        else if (isMovingDown && pos.y > minY)
        {
            currentXRot = Mathf.MoveTowards(currentXRot, minRotX, rotateSpeed * Time.deltaTime);
            pos.y = Mathf.MoveTowards(pos.y, minY, moveSpeed * Time.deltaTime);
            DragonStatusData.Instance.FlyDirection = -1;
            // animator.SetFloat("BounceSpeed", movingAniSpeed);
        }
        else
        {
            currentXRot = Mathf.MoveTowards(currentXRot, 0f, rotateSpeed * Time.deltaTime);
            DragonStatusData.Instance.FlyDirection = 0;
            // animator.SetFloat("BounceSpeed", normalAniSpeed);
        }

        rotateSubject.localEulerAngles = new Vector3(currentXRot, rot.y, rot.z);
        moveSubject.position = pos;
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
    void CheckTargetMotion()
    {
        float boxRotX = NormalizeAngle(motionTarget.eulerAngles.x);
        if (targetRotReverse) boxRotX *= -1;

        if (boxRotX > thresholdAngle)
        {
            upTimer += Time.deltaTime;
            downTimer = 0;

            if (upTimer >= holdTime)
            {
                isMovingUp = true;
                isMovingDown = false;
            }
        }
        else if (boxRotX < -thresholdAngle)
        {
            downTimer += Time.deltaTime;
            upTimer = 0;

            if (downTimer >= holdTime)
            {
                isMovingDown = true;
                isMovingUp = false;
            }
        }
        else
        {
            upTimer = 0;
            downTimer = 0;
            isMovingUp = false;
            isMovingDown = false;
        }
    }
}
