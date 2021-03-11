using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;
#pragma warning disable 649
    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField]
    private AudioSource _oneShotSource;
#pragma warning restore 649
    public AudioClip BtnClip;

    public void PlayClickSound()
    {
        _oneShotSource.PlayOneShot(BtnClip);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            _musicSource.mute = !_musicSource.mute;
        }
    }

}
