using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapEdgeIcon : MonoBehaviour
{
    public Image iconImage;

    private MinimapIcon minimapIcon;

    private RectTransform _rectTransform;
    private RectTransform _parentRectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentRectTransform = _rectTransform.parent.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var viewportPoint = minimapIcon.GetViewportPoint();
        viewportPoint.z = 0;

        var centerPoint = new Vector3(0.5f, 0.5f, 0);

        var dir = viewportPoint - centerPoint;
        dir.Normalize();

        UpdateSpriteFromMinimapIcon();
        MoveToEdge(dir);
    }

    public void Init(MinimapIcon icon)
    {
        minimapIcon = icon;
        Update();
    }

    public void UpdateSpriteFromMinimapIcon()
    {
        iconImage.sprite = minimapIcon.spriteRenderer.sprite;
        iconImage.color = minimapIcon.spriteRenderer.color;
    }

    public void MoveToEdge(Vector3 dir)
    {
        var targetPos = dir;
        targetPos.x *= _parentRectTransform.rect.width / 2;
        targetPos.y *= _parentRectTransform.rect.height / 2;
        targetPos.z = _rectTransform.localPosition.z;

        _rectTransform.localPosition = targetPos;
    }
}
