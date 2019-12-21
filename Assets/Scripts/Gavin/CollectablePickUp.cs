using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePickUp : MonoBehaviour
{
    public Interactable interactable;

    void Update()
    {
        if (interactable.canPress && Input.GetKeyDown(KeyCode.E)) 
        {
            //Add name or just increment number of collectables in Game Manager.
            Debug.Log(gameObject.name + " has been collected!");
            Destroy(gameObject);
        }
    }
}
