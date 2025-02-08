using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectBehaviour : MonoBehaviour
{
    public float Speed;
    public Vector3 MovingDir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(MovingDir * Speed * Time.deltaTime, Space.World);
    }
}
