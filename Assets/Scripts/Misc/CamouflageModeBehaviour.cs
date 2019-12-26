using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamouflageModeBehaviour : MonoBehaviour
{
    public Material materialForCamouflageMode;

    public Dictionary<Renderer, Material[]> savedMaterials = new Dictionary<Renderer, Material[]>();

    private List<Renderer> renderers = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        InitRenderers();
        SaveOriginalMaterials();

        GameManager.Instance.playerModel.camouflageModeController.onCamouflageModeActivated +=
            ApplyCamouflageModeMaterial;
        GameManager.Instance.playerModel.camouflageModeController.onCamouflageModeDeactivated +=
            RevertToOriginalMaterials;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        GameManager.Instance.playerModel.camouflageModeController.onCamouflageModeActivated -=
            ApplyCamouflageModeMaterial;
        GameManager.Instance.playerModel.camouflageModeController.onCamouflageModeDeactivated -=
            RevertToOriginalMaterials;
    }

    void InitRenderers()
    {
        renderers.Clear();

        var selfRenderer = GetComponent<Renderer>();
        if (selfRenderer != null)
        {
            renderers.Add(selfRenderer);
        }

        renderers.AddRange(GetComponentsInChildren<Renderer>());
    }

    void SaveOriginalMaterials()
    {
        savedMaterials.Clear();
        foreach (var renderer in renderers)
        {
            savedMaterials[renderer] = renderer.materials;
        }
    }

    void RevertToOriginalMaterials()
    {
        foreach (var renderer in renderers)
        {
            if (savedMaterials.ContainsKey(renderer))
            {
                renderer.materials = savedMaterials[renderer];
            }
        }
    }

    void ApplyCamouflageModeMaterial()
    {
        foreach (var renderer in renderers)
        {
            if (savedMaterials.ContainsKey(renderer))
            {
                Material[] newMaterials = new Material[renderer.materials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = materialForCamouflageMode;
                }

                renderer.materials = newMaterials;
            }
        }
    }
}
