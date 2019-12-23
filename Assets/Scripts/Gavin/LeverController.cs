using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Interactable interactable;
    public Material greenMat;
    public bool isOn = false;

    MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
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
        }
    }
}
