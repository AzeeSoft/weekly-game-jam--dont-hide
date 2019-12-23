using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusicManager : SingletonMonoBehaviour<MusicManager>
{
    public AudioSource basicAudioSource;
    public AudioSource advancedAudioSource;

    public float fadeIn;
    public float fadeOut;

    new void Awake()
    {
        base.Awake();
        
        basicAudioSource.Play();
        advancedAudioSource.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!GameManager.Instance.playerModel.isCamouflaged)
            {
                advancedAudioSource.DOFade(basicAudioSource.volume - 0.2f, fadeIn);
            }
            else
            {
                advancedAudioSource.DOFade(0, fadeOut);
            }
        }
    }
}
