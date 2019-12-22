using System.Collections;
using System.Collections.Generic;
using Cinemachine.PostFX;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CrossPlatformInput;

public class CamouflageModeController : MonoBehaviour
{
    public bool isCamouflaged { get; private set; } = false;
    public float timeOfLastCamouflage { get; private set; } = 0;
    public float timeSinceLastCamouflage => Time.time - timeOfLastCamouflage;
    public float camouflageEffectRadius => camouflageValue * camouflageRadiusMultiplier;

    public GameObject avatarRoot;
    public float camouflageEffectTransitionDuration = 2f;
    public float camouflageRadiusMultiplier = 10f;
    public PostProcessProfile camouflageModePostProcessingProfile;
    [Range(0, 1)] public float camouflageValue = 0;

    private List<Material> materials = new List<Material>();
    private Tween camouflageTransitionTween;
    private CinemachinePostProcessing thirdPersonPostProcessing;
    private PostProcessProfile originalPostProcessingProfile;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTimeOfLastCamouflage();
        FindMaterials();
        thirdPersonPostProcessing =
            CinemachineCameraManager.Instance.GetCameraByState(CinemachineCameraManager.CinemachineCameraState
                .ThirdPerson).GetComponent<CinemachinePostProcessing>();
        originalPostProcessingProfile = thirdPersonPostProcessing.m_Profile;
    }

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("CamouflageToggle"))
        {
            ToggleCamouflage();
        }

        if (isCamouflaged)
        {
            UpdateTimeOfLastCamouflage();
        }

        UpdateCamouflageValue();
    }

    public void ToggleCamouflage()
    {
        isCamouflaged = !isCamouflaged;

        AnimateCamouflageValue(isCamouflaged ? 1 : 0, camouflageEffectTransitionDuration);
        thirdPersonPostProcessing.m_Profile = isCamouflaged ? camouflageModePostProcessingProfile : originalPostProcessingProfile;
    }

    private void UpdateTimeOfLastCamouflage()
    {
        timeOfLastCamouflage = Time.time;
    }

    private void FindMaterials()
    {
        materials.Clear();
        foreach (var renderer in avatarRoot.GetComponentsInChildren<Renderer>())
        {
            materials.AddRange(renderer.materials);
        }
    }

    private void AnimateCamouflageValue(float endValue, float duration)
    {
        camouflageTransitionTween?.Kill();

        camouflageTransitionTween = DOTween.To(() => camouflageValue, value =>
        {
            camouflageValue = value;
        }, endValue, duration);
        camouflageTransitionTween.Play();
    }

    private void UpdateCamouflageValue()
    {
        foreach (var material in materials)
        {
            material.SetFloat("_CamouflageValue", camouflageValue);
        }
    }
}