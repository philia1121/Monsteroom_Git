using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AimIK : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    [Range(0, 1)]
    public float LookWeight = 1;
    public Transform AimingIK;
    public float SideLockAngle = 90f;
    public float SmoothSpeed = 5f;
    void LateUpdate()
    {
        // Quaternion targetRot = Quaternion.LookRotation(Target.position - AimingIK.position);
        // targetRot *= Quaternion.Euler(Offset);
        // AimingIK.rotation = Quaternion.Slerp(AimingIK.rotation, targetRot, LookWeight);

        // Method B
        Vector3 localTargetPos = AimingIK.InverseTransformPoint(Target.position);
        // Debug.Log(localTargetPos);

        // 判斷 target 在前方還是後方
        if (localTargetPos.y < 0) // z < 0 表示在頭部後方
        {
            // Debug.Log("At Back");
            // 判斷更靠近左還是右
            if (localTargetPos.x >= 0)
            {
                // 靠右 → 鎖在 3 點鐘方向
                localTargetPos = Quaternion.Euler(0, SideLockAngle, 0) * Vector3.forward * 1f;
            }
            else
            {
                // 靠左 → 鎖在 9 點鐘方向
                localTargetPos = Quaternion.Euler(0, -SideLockAngle, 0) * Vector3.forward * 1f;
            }

            // 轉回 world position
            Vector3 fakeWorldPos = AimingIK.TransformPoint(localTargetPos);
            ApplyIK(fakeWorldPos);
        }
        else
        {
            // 正常在前方 → 直接追蹤 target
            ApplyIK(Target.position);
        }
    }

    void ApplyIK(Vector3 worldPos)
    {
        Quaternion targetRot = Quaternion.LookRotation(worldPos - AimingIK.position);
        targetRot *= Quaternion.Euler(Offset);
        AimingIK.rotation = Quaternion.Slerp(AimingIK.rotation, targetRot, LookWeight);
    }
    public void ChangeIKTarget(Transform newTarget)
    {
        Target = newTarget;
    }
    public void ChangeAimingIK(Transform ik)
    {
        AimingIK = ik;
    }
    public void SetIKWeight(float value)
    {
        LookWeight = value;
    }
    public void SmoothSetIKeight(float value)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothLookWeight(LookWeight, value));
    }
    IEnumerator SmoothLookWeight(float current, float target)
    {
        var t = 0f;
        while (t < 1)
        {
            LookWeight = Mathf.Lerp(current, target, t);
            t += Time.deltaTime * SmoothSpeed;
            yield return null;
        }
        LookWeight = target;
    }
}
