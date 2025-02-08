using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public string targetTag = "scene";
    public bool ToDestroy = false;
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Enter");
        if(other.gameObject.tag == targetTag)
        {
            if(ToDestroy)
            {
                Destroy(other.gameObject);
            }
            else
            {
                other.gameObject.SetActive(false);
                other.GetComponent<SceneObjectBehaviour>().enabled = false;
            }
        }
    }
}
