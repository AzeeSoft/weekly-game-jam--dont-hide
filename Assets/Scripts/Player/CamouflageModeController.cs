using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CamouflageModeController : MonoBehaviour
{
    public bool isCamouflaged { get; private set; } = false;
    public float timeOfLastCamouflage { get; private set; } = 0;
    public float timeSinceLastCamouflage => Time.time - timeOfLastCamouflage;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTimeOfLastCamouflage();
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
    }

    public void ToggleCamouflage()
    {
        isCamouflaged = !isCamouflaged;
    }

    private void UpdateTimeOfLastCamouflage()
    {
        timeOfLastCamouflage = Time.time;
    }
}
