using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textMesh;
    public string text;

    [HideInInspector] public bool canPress = false;

    private void Awake()
    {
        textMesh.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textMesh.text = text;
            canPress = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textMesh.text = "";
            canPress = false;
        }
    }

    private void OnDestroy()
    {
        textMesh.text = "";
    }
}
