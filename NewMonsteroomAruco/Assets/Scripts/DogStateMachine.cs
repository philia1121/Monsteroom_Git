using UnityEngine;

public class DogStateMachine : MonoBehaviour
{
    [SerializeField] private Animator myAnimator;
    [SerializeField] private RandomInState[] allRandomInState;
    void Update()
    {
        AnimatorStateInfo stateInfo = myAnimator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(FirstDecimalDigit(stateInfo.normalizedTime));
    }
    int FirstDecimalDigit(float value) { return (int)(Mathf.Abs(value * 10) % 10); }
}

[System.Serializable]
public class RandomInState
{
    public string stateName;
    public string parameters;
    public int randomCount;
    public int currentIndex = 0;
}
