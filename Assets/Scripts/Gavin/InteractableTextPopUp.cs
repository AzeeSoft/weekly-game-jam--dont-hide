using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTextPopUp : SingletonMonoBehaviour<InteractableTextPopUp>
{
    public TMPro.TextMeshProUGUI textMesh;

    new void Awake()
    {
        base.Awake();

        textMesh.text = "";
    }
}
