using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;


    public AudioClip BtnClip;

    public AudioClip SoundClip;

    void OnEnable()
    {

        PlayAudio(SoundClip);
    }
    public void PlayAudio(AudioClip clip)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void OnDestroy()
    {
        Instance = null;
    }

}
