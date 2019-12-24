using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }

    public bool showOnEdge = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showOnEdge)
        {
            if (IsShownInMap())
            {
                Minimap.Instance.RemoveFromEdgeIcons(this);
            }
            else
            {
                Minimap.Instance.AddToEdgeIcons(this);
            }
        }
    }

    public bool IsShownInMap()
    {
        var viewportPoint = GetViewportPoint();
        viewportPoint.z = 0;

        var centerPoint = new Vector3(0.5f, 0.5f, 0);
        var dist = Vector3.Distance(centerPoint, viewportPoint);

        return dist <= 0.5f;
    }

    public Vector3 GetViewportPoint()
    {
        var minimapCam = MinimapCamera.Instance.camera;
        return minimapCam.WorldToViewportPoint(transform.position);
    }
}
