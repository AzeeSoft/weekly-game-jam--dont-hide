using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenFader : SingletonMonoBehaviour<ScreenFader>
{
    public Image image;
    public float fadeDuration = 1f;
    public bool fadeInOnAwake = true;

    public UnityEvent onFadeIn;
    public UnityEvent onFadeOut;

    public event Action onFadeInOneShot;
    public event Action onFadeOutOneShot;

    private Animator animator;
    
    new void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
        if (fadeInOnAwake)
        {
            FadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeInAndExecute(Action callback, float speedMultiplier = 1)
    {
        onFadeInOneShot += callback;
        FadeIn(speedMultiplier);
    }

    public void FadeOutAndExecute(Action callback, float speedMultiplier = 1)
    {
        onFadeOutOneShot += callback;
        FadeOut(speedMultiplier);
    }

    public void FadeIn(float speedMultiplier = 1)
    {
        animator.SetFloat("FadeSpeed", speedMultiplier);
        animator.SetTrigger("FadeIn");
    }

    public void FadeOut(float speedMultiplier = 1)
    {
        animator.SetFloat("FadeSpeed", speedMultiplier);
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeIn()
    {
        onFadeIn?.Invoke();
        onFadeInOneShot?.Invoke();
        onFadeInOneShot = null;
    }

    public void OnFadeOut()
    {
        onFadeOut?.Invoke();
        onFadeOutOneShot?.Invoke();
        onFadeOutOneShot = null;
    }
}
