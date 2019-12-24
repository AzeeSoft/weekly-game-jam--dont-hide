using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Interactable interactable;
    public Material greenMat;
    public bool isOn = false;

    MeshRenderer mr;
    private MinimapIcon minimapIcon;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        minimapIcon = GetComponentInChildren<MinimapIcon>();
    }

    void Update()
    {
        if (interactable.canPress && Input.GetKeyDown(KeyCode.E) && !isOn)
        {
            Debug.Log(gameObject.name + " has been switched!");
            mr.material = greenMat;
            isOn = true;

            interactable.isPressed = true;
            interactable.TurnOffText();

            minimapIcon.gameObject.SetActive(false);

            GameManager.Instance.OnLeverUsed(this);
        }
    }
}
