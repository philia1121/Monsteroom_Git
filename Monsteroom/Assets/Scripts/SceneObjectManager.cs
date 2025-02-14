using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class SceneObjectManager : MonoBehaviour
{
    [Header("Prefabs Settings")]
    [Tooltip("場景物件Prebab")]public GameObject[] SceneObjects;

    [Header("Pool Settings")]
    [Tooltip("場景物件 生成池父物件")]public Transform SceneObjectPool;
    List<GameObject> pooledObjects = new List<GameObject>();
    [Tooltip("場景物件 預生成數/每種 \n 共生成AmountForEach*種類數 個場景物件Prefab")]public int AmountForEach = 10;

    [Header("Pool Settings")]
    [Tooltip("生成開關 //沒事可以不要動它")]public bool Spawn = true;
    [Tooltip("場景物件 隨機生成的參考座標 \n 共4個,前兩個是鏡頭偏左,後兩個是鏡頭偏右,避免生成的物件穿過鏡頭")]public Transform[] RefTransforms;
    [Tooltip("場景物件 隨機尺寸大小值")]public float SizeMin, SizeMax;
    [Tooltip("初始可見的場景物件數量")]public int initialAmount = 10;
    [Tooltip("隨機生成間隔大小值(秒)")]public float SpawnTimeMin, SpawnTimeMax;
    void OnEnable()
    {
        InitailizeObjectPool();
        InitailizeScene();

        StartCoroutine(RandomSpawn());
    }

    void InitailizeObjectPool()
    {
        foreach(var item in SceneObjects)
        {
            for(int i = 0; i < AmountForEach; i++)
            {
                var newSpawned = Instantiate(item, Vector3.zero, item.transform.rotation, SceneObjectPool);
                newSpawned.SetActive(false);
                pooledObjects.Add(newSpawned);
            }
        }
    }
    public GameObject GetPooledObject()
    {
        var usableObjects = new List<GameObject>();
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].gameObject.activeSelf)
            {
                usableObjects.Add(pooledObjects[i]);
            }
        }

        if(usableObjects.Count > 0)
        {
            return usableObjects[Random.Range(0, usableObjects.Count)];
        }
        else
        {
            return null;
        }
    }

    IEnumerator RandomSpawn()
    {
        while(Spawn)
        {
            var obj = GetPooledObject();
            if(!obj)
            {
                yield return new WaitForSeconds(Random.Range(SpawnTimeMin, SpawnTimeMax));
            }
            SetObjectActive(obj);

            yield return new WaitForSeconds(Random.Range(SpawnTimeMin, SpawnTimeMax));
        }
        yield return null;
    }

    void InitailizeScene()
    {
        for(int i = 0; i < initialAmount; i++)
        {
            var obj = GetPooledObject();
            SetObjectActive(obj);
        }
    }

    void SetObjectActive(GameObject m_obj)
    {
        bool refGroup = (Random.Range(0, 1f) > 0.5f)? true : false;
        var pos1 = refGroup? RefTransforms[0].position: RefTransforms[2].position;
        var pos2 = refGroup? RefTransforms[1].position: RefTransforms[3].position;
        Vector3 spawnPos = new Vector3(Random.Range(pos1.x, pos2.x), Random.Range(pos1.y, pos2.y), Random.Range(pos1.z, 0));

        m_obj.transform.position = spawnPos;
        m_obj.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360f), 0));
        m_obj.transform.localScale = new Vector3(Random.Range(SizeMin, SizeMax), Random.Range(SizeMin, SizeMax), Random.Range(SizeMin, SizeMax));
        m_obj.SetActive(true);
        var temp = m_obj.GetComponent<SceneObjectBehaviour>();
        temp.enabled = true;
    }
}
