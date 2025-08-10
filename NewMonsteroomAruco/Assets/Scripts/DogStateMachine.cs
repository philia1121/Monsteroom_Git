using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class DogStateMachine : MonoBehaviour
{
    [SerializeField] private Animator myAnimator;
    [SerializeField] private RandomInState[] allRandomInState;
    bool needRandomState = false;
    RandomInState currentState;
    bool randomized = false;
    void Awake()
    {

    }
    void Update()
    {
        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);
        foreach (var item in allRandomInState)
        {
            if (stateInfo.IsName(item.stateName))
            {
                needRandomState = true;
                currentState = item;
            }
        }
        if (needRandomState)
        {
            RandomizeInState(stateInfo);
        }
    }
    void RandomizeInState(AnimatorStateInfo info)
    {
        if (FirstDecimalDigit(info.normalizedTime) < 50 && randomized) randomized = false;
        if (FirstDecimalDigit(info.normalizedTime) > 95 && !randomized)
        {
            Debug.Log("Start Randomize");
            int nextIndex;
            if (currentState.blendThreshold.Length > 2)
            {
                var rd = UnityEngine.Random.Range(0, 1f);
                var doRandom = rd < 0.9f ? false : true;
                if (doRandom)
                {
                    do
                    {
                        nextIndex = UnityEngine.Random.Range(0, currentState.blendThreshold.Length);
                    }
                    while (nextIndex == currentState.currentIndex);
                }
                else
                {
                    nextIndex = 0;
                }
            }
            else
            {
                var rd = UnityEngine.Random.Range(0, 1f);
                nextIndex = rd < 0.9f ? 0 : 1;
            }

            currentState.currentIndex = nextIndex;
            // myAnimator.SetFloat(currentState.parameters, currentState.blendThreshold[nextIndex]);
            StartCoroutine(SmoothParameter(currentState.parameters, myAnimator.GetFloat(currentState.parameters), currentState.blendThreshold[nextIndex]));
            Debug.Log("Randomized: " + currentState.stateName + ", " + currentState.currentIndex);
            randomized = true;
        }
    }
    int FirstDecimalDigit(float value) { return (int)(Mathf.Abs(value * 100) % 100); }
    IEnumerator SmoothParameter(string parameter, float current, float target)
    {
        float t = 0;
        while (t < 1)
        {
            var value = Mathf.Lerp(current, target, t);
            myAnimator.SetFloat(parameter, value);
            t += 0.5f * Time.deltaTime;
            yield return null;
        }
        myAnimator.SetFloat(parameter, target);
    }
}

[System.Serializable]
public class RandomInState
{
    public string stateName;
    public string parameters;
    public float[] blendThreshold;
    public int currentIndex = 0;
}
