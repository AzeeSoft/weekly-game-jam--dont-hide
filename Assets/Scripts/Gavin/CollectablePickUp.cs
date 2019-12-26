using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablePickUp : MonoBehaviour
{
    public string collectableName;

    [TextArea] public string collectableDescription;
    
    public Interactable interactable;
    public AudioClip collectedSound;

    void Start()
    {
        interactable.text = $"Press 'E' to collect: {collectableName}";
    }

    void Update()
    {
        if (interactable.canPress && Input.GetKeyDown(KeyCode.E)) 
        {
            //Add name or just increment number of collectables in Game Manager.
            Debug.Log(gameObject.name + " has been collected!");
            interactable.isPressed = true;
            Destroy(gameObject);

            SoundEffectsManager.Instance.PlayAt(collectedSound, transform.position);

            GameManager.Instance.OnCollectiblePickedUp(this);
        }
    }
}
