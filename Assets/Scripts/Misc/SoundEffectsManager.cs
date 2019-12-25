using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : SingletonMonoBehaviour<SoundEffectsManager>
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }

        audioSource.spatialize = false;
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayAt(AudioClip audioClip, Vector3 position)
    {
        if (audioClip == null)
        {
            return;
        }

        audioSource.spatialize = true;
        audioSource.transform.position = position;
        audioSource.PlayOneShot(audioClip);
    }
}
