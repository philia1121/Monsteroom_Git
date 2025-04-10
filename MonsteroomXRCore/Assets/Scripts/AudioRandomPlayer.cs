using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomPlayer : MonoBehaviour
{
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip[] audioClips;
    [SerializeField]private float minInterval, maxInterval;
    [SerializeField]private bool keepRandomPlay = false;
    bool randomPlay;
    IEnumerator cor;

    public void SetRandomPlay(bool play)
    {
        randomPlay = play;
        if(play)
        {
            cor = PlayRandomSound();
            StartCoroutine(cor);
        }
        else
        {
            if(cor != null)
                StopCoroutine(cor);
        }
    }
    IEnumerator PlayRandomSound()
    {
        while(randomPlay)
        {
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);
            if(randomPlay)
                PlaySound();
            if(!keepRandomPlay) randomPlay = false;
        }
    }
    void PlaySound()
    {
        if (audioClips.Length > 0)
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];

        if (audioSource.clip != null && !audioSource.isPlaying)
            audioSource.Play();
    }

}
