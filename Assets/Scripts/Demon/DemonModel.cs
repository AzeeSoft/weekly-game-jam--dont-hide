using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonModel : MonoBehaviour
{
    public bool isVisible => avatar.activeSelf;
    public bool canSeePlayer => isVisible;

    public PlayerModel playerModel { get; private set; }

    public GameObject avatar;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GameManager.Instance.playerModel;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerModel.isCamouflaged)
        {
            ShowDemon();
        }
        else
        {
            HideDemon();
        }
    }

    private void ShowDemon()
    {
        if (!isVisible)
        {
            avatar.SetActive(true);
        }
    }

    private void HideDemon()
    {
        if (isVisible)
        {
            avatar.SetActive(false);
        }
    }
}
