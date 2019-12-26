using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public TextMeshProUGUI collectableListTitle;
    public Transform collectableListRoot;
    public GameObject collectableUIItemPrefab;
    public TextMeshProUGUI descriptionText;

    public InventoryRenderer inventoryRenderer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        inventoryRenderer.RenderPrefab(null);

        List<string> collectablesCollected = GameManager.Instance.collectablesCollected.ToList();

        int totalCollectables =
            Mathf.Max(GameManager.Instance.collectablePrefabsDict.Count, collectablesCollected.Count);
        collectableListTitle.text = $"Collectables ({collectablesCollected.Count}/{totalCollectables})";

        collectableListRoot.DestroyAllChildren();
        foreach (var collectable in collectablesCollected)
        {
            var collectableUIItem = Instantiate(collectableUIItemPrefab, collectableListRoot);

            var collectableText = collectableUIItem.GetComponentInChildren<TextMeshProUGUI>();
            collectableText.text = collectable;

            var collectableButton = collectableUIItem.GetComponentInChildren<Button>();
            collectableButton.onClick.AddListener(() =>
            {
                OnCollectableSelected(collectable);
            });
        }

        if (collectablesCollected.Count > 0)
        {
            OnCollectableSelected(collectablesCollected[0]);
        }
    }

    void OnCollectableSelected(string collectable)
    {
        GameManager.Instance.collectablePrefabsDict.TryGetValue(collectable, out var collectablePrefab);
        inventoryRenderer.RenderPrefab(collectablePrefab);

        var collectablePickUp = collectablePrefab.GetComponent<CollectablePickUp>();
        descriptionText.text = collectablePickUp.collectableDescription;
    }
}