using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonStatusData : MonoBehaviour
{
    public static DragonStatusData Instance { get; private set; }
    public int FlyDirection = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
