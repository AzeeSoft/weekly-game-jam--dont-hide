using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : SingletonMonoBehaviour<Minimap>
{
    public GameObject edgeIconsContainer;
    public GameObject edgeIconPrefab;

    private Dictionary<MinimapIcon, MinimapEdgeIcon> edgeIcons = new Dictionary<MinimapIcon, MinimapEdgeIcon>();

    new void Awake()
    {
        base.Awake();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToEdgeIcons(MinimapIcon icon)
    {
        if (!edgeIcons.ContainsKey(icon))
        {
            var edgeIcon = Instantiate(edgeIconPrefab, edgeIconsContainer.transform).GetComponent<MinimapEdgeIcon>();
            edgeIcon.Init(icon);

            edgeIcons[icon] = edgeIcon;
        }
    }

    public void RemoveFromEdgeIcons(MinimapIcon icon)
    {
        if (edgeIcons.ContainsKey(icon))
        {
            Destroy(edgeIcons[icon].gameObject);
            edgeIcons.Remove(icon);
        }
    }
}