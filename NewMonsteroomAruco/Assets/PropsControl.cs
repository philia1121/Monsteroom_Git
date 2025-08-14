using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
public class PropsControl : MonoBehaviour
{
    MyInputMap myInputMap;
    public GameObject[] propsInHand;
    int currentInHand = 0;
    public DogProps[] dogProps;
    public AimIK dogIK;
    public GameObject toyInMouth;
    public GameObject[] fakeProps;

    public UnityEvent OnSwitchProps, OnDropProps, OnPickProps, OnGuarding, OffGuarding;
    public bool dogOccupied = false;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.Interaction.Enable();
        myInputMap.Interaction.Switch.started += ctx => SwitchProps();
        myInputMap.Interaction.Put.started += ctx => DropAndPickProps();
    }
    void Start()
    {
        foreach (var item in propsInHand)
        {
            item.SetActive(false);
        }
        propsInHand[currentInHand].SetActive(true);
        OnSwitchProps.Invoke();
    }
    void SwitchProps()
    {
        propsInHand[currentInHand].SetActive(false);
        currentInHand += 1;
        if (currentInHand > propsInHand.Length - 1) currentInHand = 0;

        propsInHand[currentInHand].SetActive(true);
    }
    void DropAndPickProps()
    {
        if (dogProps[currentInHand].inHand)
        {
            dogProps[currentInHand].prefab.transform.position = propsInHand[currentInHand].transform.position;
            dogProps[currentInHand].prefab.SetActive(true);
            dogProps[currentInHand].inHand = false;
            OnDropProps.Invoke();
        }
        else if (!dogProps[currentInHand].inHand && !dogProps[currentInHand].interacting)
        {
            OnPickProps.Invoke();
            StartCoroutine(SmoothPick(currentInHand, dogProps[currentInHand].prefab.transform.position, propsInHand[currentInHand].transform.position));
        }
    }
    public void SetPropsInteractingState(int index)
    {
        dogProps[index].interacting = true;
    }
    IEnumerator SmoothPick(int index, Vector3 now, Vector3 target)
    {
        var t = 0f;
        dogProps[index].interactRange.enabled = false;
        var originalScale = dogProps[index].prefab.transform.localScale;
        while (t < 1)
        {
            dogProps[index].prefab.transform.position = Vector3.Lerp(now, target, t);
            dogProps[index].prefab.transform.localScale = Vector3.Lerp(originalScale, propsInHand[index].transform.localScale, t);
            t += Time.deltaTime * 15;
            yield return null;
        }
        dogProps[index].prefab.SetActive(false);
        dogProps[index].prefab.transform.position = target;
        dogProps[index].prefab.transform.localScale = originalScale;
        dogProps[index].inHand = true;
        dogProps[index].interacting = false;
        dogProps[index].interactRange.enabled = true;
    }
    public void TurnOnFakeProps(int index)
    {
        dogProps[index].prefab.SetActive(false);
        dogProps[index].interacting = true;
        fakeProps[index].SetActive(true);

        switch (index)
        {
            case 0:
                // toyInMouth.SetActive(false);
                break;
            case 1:
                // toyInMouth.SetActive(false);
                break;
            case 2:
                // toyInMouth.SetActive(false);
                break;
        }
    }
    public void ReleaseProps(int index)
    {
        dogProps[index].prefab.SetActive(false);
        dogProps[index].inHand = true;
        dogProps[index].interacting = false;
        dogProps[index].interactRange.enabled = true;
        fakeProps[index].SetActive(false);
    }
    public void SetDogOccupiedState(bool occupied)
    {
        dogOccupied = occupied;
        foreach (var item in dogProps)
        {
            item.interactRange.enabled = !occupied;
        }
        CheckOccupiedReaction();
    }
    public void CheckOccupiedReaction()
    {
        if (dogOccupied)
        {
            OnGuarding.Invoke();
        }
        else
        {
            OffGuarding.Invoke();
        }
    }
}
[System.Serializable]
public class DogProps
{
    public GameObject prefab;
    public Collider interactRange;
    public bool inHand;
    public bool interacting;
}

