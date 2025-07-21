using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PetDogDemo : MonoBehaviour
{
    [SerializeField] private bool RHandInRange;
    [SerializeField] private bool LHandInRange;
    [Header("")]
    [SerializeField] private float petRequireDuration;
    [SerializeField] private ParticleSystem effect;
    private float petDuration;
    public UnityEvent IsPettingEvent, IsNotPettingEvent, IsPetEnoughEvent;
    [SerializeField] private bool showLog = false;
    bool wasPetting;
    public void SetRHandInRange(bool value) { RHandInRange = value; }
    public void SetLHandInRange(bool value) { LHandInRange = value; }

    void Start()
    {
        wasPetting = RHandInRange | LHandInRange;
    }

    public void CheckPettingState()
    {
        var isPetting = RHandInRange | LHandInRange;
        if (isPetting != wasPetting)
        {
            if (isPetting)
            {
                IsPettingEvent.Invoke();
                effect.Play();
                var main = effect.main;
                main.loop = true;
                if (showLog) Debug.Log("isPetting Triggered");
            }
            else
            {
                IsNotPettingEvent.Invoke();
                var main = effect.main;
                main.loop = false;
                if (showLog) Debug.Log("isNotPetting Triggered");
            }
        }

        wasPetting = RHandInRange | LHandInRange;
    }
    void Update()
    {
        petDuration += Time.deltaTime;
        if (petDuration > petRequireDuration)
        {
            this.enabled = false;
            var main = effect.main;
            main.loop = false;
            IsPetEnoughEvent.Invoke();
        }
    }
}
