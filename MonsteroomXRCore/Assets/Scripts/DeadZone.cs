using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    [SerializeField]private string targetTag = "scene";
    [SerializeField]private bool toDestroy = false;
    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Enter");
        if(other.gameObject.tag == targetTag)
        {
            if(toDestroy)
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
