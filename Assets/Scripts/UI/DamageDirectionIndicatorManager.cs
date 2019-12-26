using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDirectionIndicatorManager : SingletonMonoBehaviour<DamageDirectionIndicatorManager>
{
    public enum IndicatorType
    {
        Enemy,
        Demon,
    }

    public Transform container;
    public GameObject enemyIndicatorPrefab;
    public GameObject demonIndicatorPrefab;

    private Dictionary<Transform, DamageDirectionIndicator> trackingTransforms = new Dictionary<Transform, DamageDirectionIndicator>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IndicateDamageFrom(Transform otherTransform, IndicatorType type)
    {
        if (trackingTransforms.ContainsKey(otherTransform))
        {
            trackingTransforms[otherTransform].ResetTimer();
            return;
        }

        GameObject indicatorPrefab = null;
        switch (type)
        {
            case IndicatorType.Enemy:
                indicatorPrefab = enemyIndicatorPrefab;
                break;
            case IndicatorType.Demon:
                indicatorPrefab = demonIndicatorPrefab;
                break;
        }

        var damageDirIndicator = Instantiate(indicatorPrefab, container).GetComponent<DamageDirectionIndicator>();
        damageDirIndicator.trackingTransform = otherTransform;

        trackingTransforms[otherTransform] = damageDirIndicator;
    }

    public void StopTrackingTransform(Transform otherTransform)
    {
        trackingTransforms.Remove(otherTransform);
    }
}
