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
    public AudioClip camoOnSound;
    public AudioClip camoOffSound;
    public float camouflageEffectTransitionDuration = 2f;
    public float camouflageRadiusMultiplier = 10f;
    public PostProcessProfile camouflageModePostProcessingProfile;
    [Range(0, 1)] public float camouflageValue = 0;

    private PlayerModel playerModel;
    private List<Material> materials = new List<Material>();
    private Tween camouflageTransitionTween;
    private CinemachinePostProcessing thirdPersonPostProcessing;
    private PostProcessProfile originalPostProcessingProfile;

    // Start is called before the first frame update
    void Start()
    {
        playerModel = GetComponent<PlayerModel>();

        UpdateTimeOfLastCamouflage();
        FindMaterials();
        thirdPersonPostProcessing =
            CinemachineCameraManager.Instance.GetCameraByState(CinemachineCameraManager.CinemachineCameraState
                .ThirdPerson).GetComponent<CinemachinePostProcessing>();
        originalPostProcessingProfile = thirdPersonPostProcessing.Profile;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerModel.isDead)
        {
            return;
        }

        if (playerModel.playerInputController.playerInput.ToggleCamouflageMode)
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

        playerModel.animator.SetTrigger(isCamouflaged ? "EnableCamouflage" : "DisableCamouflage");
        SoundEffectsManager.Instance.Play(isCamouflaged ? camoOnSound : camoOffSound);
        AnimateCamouflageValue(isCamouflaged ? 1 : 0, camouflageEffectTransitionDuration, () => { });
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

    private void AnimateCamouflageValue(float endValue, float duration, TweenCallback callback)
    {
        camouflageTransitionTween?.Kill();

        camouflageTransitionTween =
            DOTween.To(() => camouflageValue, value => { camouflageValue = value; }, endValue, duration);
        camouflageTransitionTween.onComplete += callback;
        camouflageTransitionTween.Play();
    }

    private void UpdateCamouflageValue()
    {
        foreach (var material in materials)
        {
            material.SetFloat("_CamouflageValue", camouflageValue);
        }

        if (thirdPersonPostProcessing.Profile.HasSettings<ColorGrading>())
        {
            var colorGrading = thirdPersonPostProcessing.Profile.GetSetting<ColorGrading>();
            var origColorGrading = originalPostProcessingProfile.GetSetting<ColorGrading>();
            var camouflageColorGrading = camouflageModePostProcessingProfile.GetSetting<ColorGrading>();

            float curPostExposure = HelperUtilities.Remap(camouflageValue, 0, 1, origColorGrading.postExposure,
                camouflageColorGrading.postExposure);
            colorGrading.postExposure.value = curPostExposure;

            float curRedOutRedIn = HelperUtilities.Remap(camouflageValue, 0, 1, origColorGrading.mixerRedOutRedIn,
                camouflageColorGrading.mixerRedOutRedIn);
            colorGrading.mixerRedOutRedIn.value = curRedOutRedIn;
            float curRedOutGreenIn = HelperUtilities.Remap(camouflageValue, 0, 1, origColorGrading.mixerRedOutGreenIn,
                camouflageColorGrading.mixerRedOutGreenIn);
            colorGrading.mixerRedOutGreenIn.value = curRedOutGreenIn;
            float curRedOutBlueIn = HelperUtilities.Remap(camouflageValue, 0, 1, origColorGrading.mixerRedOutBlueIn,
                camouflageColorGrading.mixerRedOutBlueIn);
            colorGrading.mixerRedOutBlueIn.value = curRedOutBlueIn;
        }

        if (thirdPersonPostProcessing.Profile.HasSettings<Vignette>())
        {
            var vignette = thirdPersonPostProcessing.Profile.GetSetting<Vignette>();
            var origVignette = originalPostProcessingProfile.GetSetting<Vignette>();
            var camouflageVignette = camouflageModePostProcessingProfile.GetSetting<Vignette>();

            float curVignetteIntensity = HelperUtilities.Remap(camouflageValue, 0, 1, origVignette.intensity,
                camouflageVignette.intensity);
            vignette.intensity.value = curVignetteIntensity;

            float curVignetteSmoothness = HelperUtilities.Remap(camouflageValue, 0, 1, origVignette.smoothness,
                camouflageVignette.smoothness);
            vignette.smoothness.value = curVignetteSmoothness;
        }

        if (thirdPersonPostProcessing.Profile.HasSettings<ChromaticAberration>())
        {
            var chromaticAberration = thirdPersonPostProcessing.Profile.GetSetting<ChromaticAberration>();
            var origChromaticAberration = originalPostProcessingProfile.GetSetting<ChromaticAberration>();
            var camouflageChromaticAberration = camouflageModePostProcessingProfile.GetSetting<ChromaticAberration>();

            float curChromaticAbberationIntensity =
                HelperUtilities.Remap(camouflageValue, 0, 1, origChromaticAberration.intensity,
                    camouflageChromaticAberration.intensity);
            chromaticAberration.intensity.value = curChromaticAbberationIntensity;
        }
    }
}