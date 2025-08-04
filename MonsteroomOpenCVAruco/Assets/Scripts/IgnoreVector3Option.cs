using UnityEngine;
[System.Serializable]
public class IgnoreVector3Option
{
    public bool ignoreX, ignoreY, ignoreZ;
    public static Vector3 FilteredPosition(IgnoreVector3Option ignore, Vector3 value, bool ignoreUsingOriginal = false)
    {
        float newX = ignore.ignoreX ? 0 : value.x;
        float newY = ignore.ignoreY ? 0 : value.y;
        float newZ = ignore.ignoreZ ? 0 : value.z;
        return new Vector3(newX, newY, newZ);
    }
    public static Quaternion FilteredRotation(IgnoreVector3Option ignore, Quaternion value, bool ignoreUsingOriginal = false)
    {
        var rot = value.eulerAngles;
        float newX = ignore.ignoreX ? 0 : rot.x;
        float newY = ignore.ignoreY ? 0 : rot.y;
        float newZ = ignore.ignoreZ ? 0 : rot.z;
        return Quaternion.Euler(new Vector3(newX, newY, newZ));
    }
}
