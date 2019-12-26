using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRenderer : MonoBehaviour
{
    public Vector3 boundsSize;
    public Transform objectHolder;
    public Animator rotationAnimator;
    public Camera camera;
    public Vector3 transportToLocation = new Vector3(10000f, 10000f, 10000f);

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(objectHolder.position, boundsSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = transportToLocation;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RenderPrefab(GameObject prefab)
    {
        objectHolder.DestroyAllChildren();
        objectHolder.rotation = Quaternion.identity;

        if (prefab != null)
        {
            var instance = Instantiate(prefab, objectHolder);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;

            var instanceBounds = instance.transform.GetBoundsFromRenderers();

            var scale = new Vector3(boundsSize.x / instanceBounds.size.x, boundsSize.y / instanceBounds.size.y,
                boundsSize.z / instanceBounds.size.z);

            var minSide = Mathf.Min(scale.x, scale.y, scale.z);
            scale = new Vector3(minSide, minSide, minSide);

            instance.transform.localScale = scale;

            instanceBounds = instance.transform.GetBoundsFromRenderers();
            instance.transform.position += (instance.transform.position - instanceBounds.center);
        }
    }
}