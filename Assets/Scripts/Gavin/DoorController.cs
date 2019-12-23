using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public LeverController[] levers;
    public Material openMat;
    MeshRenderer mr;

    public bool isOpen = false;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        LeverChecker();
    }

    void LeverChecker()
    {
        for (int i = 0; i < levers.Length; i++)
        {
            if (!levers[i].isOn)
            {
                return;
            }
        }

        if (!isOpen)
        {
            Debug.Log("Door has been open!");
            mr.material = openMat;
            isOpen = true;
        }
    }
}
