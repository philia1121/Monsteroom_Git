using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelAlignment : MonoBehaviour
{
    MyInputMap myInputMap;
    [SerializeField] private Transform target;
    [SerializeField] private Transform follower;
    [SerializeField] private bool alignPosition = true;
    [SerializeField] private bool alignRotation = true;
    [SerializeField] private IgnoreVector3Option posIgnore, rotIgnore;
    [SerializeField] private bool useSmoothDamp = false;
    [SerializeField] private float smoothTime = 0.2f;
    Vector3 velocity = Vector3.zero;
    void Start()
    {
        if (!follower)
            follower = this.transform;
    }

    void Update()
    {
        if (alignPosition)
        {
            var newPos = IgnoreVector3Option.FilteredPosition(posIgnore, target.position);
            var finalPos = new Vector3(
                posIgnore.ignoreX ? follower.position.x : newPos.x,
                posIgnore.ignoreY ? follower.position.y : newPos.y,
                posIgnore.ignoreZ ? follower.position.z : newPos.z
            );
            follower.transform.position = useSmoothDamp ? Vector3.SmoothDamp(transform.position, finalPos, ref velocity, smoothTime) : finalPos;
        }

        if (alignRotation)
        {
            var newRot = IgnoreVector3Option.FilteredRotation(rotIgnore, target.rotation).eulerAngles;
            follower.transform.rotation = Quaternion.Euler(new Vector3(
                rotIgnore.ignoreX ? follower.rotation.eulerAngles.x : newRot.x,
                rotIgnore.ignoreY ? follower.rotation.eulerAngles.y : newRot.y,
                rotIgnore.ignoreZ ? follower.rotation.eulerAngles.z : newRot.z
            ));
        }

    }
}
