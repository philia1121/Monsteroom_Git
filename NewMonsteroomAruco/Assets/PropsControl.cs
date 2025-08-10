using UnityEngine;
using UnityEngine.InputSystem;
public class PropsControl : MonoBehaviour
{
    MyInputMap myInputMap;
    public GameObject[] porpsInHand;
    int currentInHand = 0;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.Interaction.Enable();
        myInputMap.Interaction.Switch.started += ctx => SwitchProps();
    }
    void Start()
    {
        foreach (var item in porpsInHand)
        {
            item.SetActive(false);
        }
        porpsInHand[currentInHand].SetActive(true);
    }
    void SwitchProps()
    {
        porpsInHand[currentInHand].SetActive(false);
        currentInHand += 1;
        if (currentInHand > porpsInHand.Length - 1) currentInHand = 0;

        porpsInHand[currentInHand].SetActive(true);
    }
}
