using Meta.WitAi;
using Unity.VisualScripting;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float lifetime = 20;
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
