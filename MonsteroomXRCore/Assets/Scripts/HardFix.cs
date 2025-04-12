using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardFix : MonoBehaviour
{
    public void ForceFlipY(Transform target)
    {
        target.Rotate( 0, 180, 0);
    }
}

