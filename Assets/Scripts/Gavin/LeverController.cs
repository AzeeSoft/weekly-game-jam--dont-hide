using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Interactable interactable;
    public bool isOn = false;
    
    Animator animator;
    private MinimapIcon minimapIcon;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        minimapIcon = GetComponentInChildren<MinimapIcon>();
    }

    void Update()
    {
        if (interactable.canPress && Input.GetKeyDown(KeyCode.E) && !isOn)
        {
            Debug.Log(gameObject.name + " has been switched!");
            isOn = true;

            interactable.isPressed = true;
            interactable.TurnOffText();

            animator.SetTrigger("Activate");
            minimapIcon.gameObject.SetActive(false);

            GameManager.Instance.OnLeverUsed(this);
        }
    }
}
