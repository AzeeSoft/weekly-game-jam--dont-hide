using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonModel : MonoBehaviour
{
    public bool isVisible => skinnedMeshRenderer.enabled;
    public bool canSeePlayer => isVisible;

    public PlayerModel playerModel { get; private set; }

    public GameObject avatar;
    public GameObject minimapIcon;
    public AudioSource cloakRustleAudioSource;
    public AudioSource attackingAudioSource;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GameManager.Instance.playerModel;
        skinnedMeshRenderer = avatar.GetComponentInChildren<SkinnedMeshRenderer>();

        cloakRustleAudioSource.volume = 0f;
        attackingAudioSource.volume = 0f;
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
            //            avatar.SetActive(true);
            skinnedMeshRenderer.enabled = true;
            minimapIcon.SetActive(true);
            cloakRustleAudioSource.volume = 1f;
        }
    }

    private void HideDemon()
    {
        if (isVisible)
        {
//            avatar.SetActive(false);
            skinnedMeshRenderer.enabled = false;
            minimapIcon.SetActive(false);
            cloakRustleAudioSource.volume = 0f;
        }
    }
}
