using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    public AudioClip basicClip;
    public AudioClip advancedClip;

    AudioSource audSource;

    new void Awake()
    {
        base.Awake();

        audSource = GetComponent<AudioSource>();
        audSource.clip = basicClip;
        audSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!GameManager.Instance.playerModel.isCamouflaged)
            {
                float timeToStart = audSource.time;
                audSource.clip = advancedClip;
                audSource.time = timeToStart;
                audSource.Play();
            }
            else
            {
                float timeToStart = audSource.time;
                audSource.clip = basicClip;
                audSource.time = timeToStart;
                audSource.Play();
            }
        }
    }
}
