using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CloudFlyEffect : MonoBehaviour
{
    [SerializeField] private Volume cloudVolume;
    VolumeProfile cloudProfile;
    VolumetricClouds clouds;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector3 flyDirection = Vector3.forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!cloudProfile)
        {
            cloudProfile = cloudVolume.profile;
        }
        if (cloudProfile)
        {
            if (cloudProfile.TryGet(out VolumetricClouds vc))
            {
                clouds = vc;
                Debug.Log(clouds.shapeOffset);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(flyDirection.x * speed * Time.deltaTime, flyDirection.y * speed * Time.deltaTime, flyDirection.z * speed * Time.deltaTime);
        var temp = clouds.shapeOffset.value + move;
        Debug.Log(clouds.shapeOffset);
        clouds.shapeOffset.value = temp;
        clouds.shapeOffset.overrideState = true;
    }

}

