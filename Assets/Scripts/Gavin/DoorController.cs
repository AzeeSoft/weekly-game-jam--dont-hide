using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public LeverController[] levers;
    private Animator animator;

    [HideInInspector] public bool isOpen = false;

    public MinimapIcon openIcon;
    public MinimapIcon closedIcon;

    public bool advanceToNextLevel = false;
    public string nextLevel = "";

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (levers.Length == 0)
        {
            closedIcon.gameObject.SetActive(false);
        }

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

        if (levers.Length > 0 && !isOpen)
        {
            Debug.Log("Door has been open!");
            animator.SetTrigger("Open");
            isOpen = true;

            closedIcon.gameObject.SetActive(false);
            openIcon.gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (isOpen && advanceToNextLevel)
        {
            ScreenFader.Instance.FadeOutAndExecute(() => SceneManager.LoadScene(nextLevel));
        }
    }
}