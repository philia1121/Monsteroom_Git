using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
    [SerializeField]private AudioSource audioSource;
    [SerializeField]private AudioClip[] audioClips;
    public void PlayAudio_RandomPick()
    {
        audioSource.clip = audioClips[audioClips.Length == 1? 0 : Random.Range(0, audioClips.Length)];
        if(audioSource.clip != null && !audioSource.isPlaying) audioSource.Play();
    }
    public void PlayAudio_Assigned(AudioClip clip)
    {
        audioSource.clip = clip;
        if(audioSource.clip != null && !audioSource.isPlaying) audioSource.Play();
    }
}
