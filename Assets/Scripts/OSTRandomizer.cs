using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTRandomizer : MonoBehaviour
{
    public List<AudioClip> clips = new List<AudioClip>();
    AudioSource audioSource;
    int clipIndex;

    void Start ()
    {
        clipIndex = Random.Range(0, clips.Count);
        audioSource = GetComponent<AudioSource>();
        Play(clips[clipIndex]);
    }

    void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        Invoke("PlayNext", clip.length);
    }

    void PlayNext()
    {
        clipIndex = Random.Range(0, clips.Count);
        Play(clips[clipIndex]);
    }
}
