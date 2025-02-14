using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpeedManager : MonoBehaviour
{
    public static SpeedManager Instance { get; private set;}
    public float Speed { get; private set;} = 5f;
    [SerializeField]private float speedMax, speedMin;
    [SerializeField]private float dashDuration;
    [SerializeField]private float lerpDuration;
    Coroutine speedChangeRoutine;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpeedUp()
    {
        if (speedChangeRoutine != null) StopCoroutine(speedChangeRoutine);
        speedChangeRoutine = StartCoroutine(SpeedBoostRoutine());
    } 
    IEnumerator SpeedBoostRoutine()
    {
        yield return ChangeSpeed(speedMax, lerpDuration); 

        yield return new WaitForSeconds(dashDuration);

        yield return ChangeSpeed(speedMin, lerpDuration);
    }

    private IEnumerator ChangeSpeed(float targetSpeed, float duration)
    {
        float startSpeed = Speed;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            Speed = Mathf.Lerp(startSpeed, targetSpeed, time / duration);
            yield return null;
        }
        Speed = targetSpeed;
    }
}
