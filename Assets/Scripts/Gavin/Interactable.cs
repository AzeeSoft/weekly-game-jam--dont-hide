using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string text;

    [HideInInspector] public bool canPress = false;
    [HideInInspector] public bool isPressed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !isPressed)
        {
            InteractableTextPopUp.Instance.textMesh.text = text;
            canPress = true;
        }
    }

    public void TurnOffText()
    {
        InteractableTextPopUp.Instance.textMesh.text = "";
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            InteractableTextPopUp.Instance.textMesh.text = "";
            canPress = false;
        }
    }

    private void OnDestroy()
    {
        InteractableTextPopUp.Instance.textMesh.text = "";
    }
}
