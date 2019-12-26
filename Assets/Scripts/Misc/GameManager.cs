using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerPrefsCollectables
{
    public List<string> collectables;
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public int leversUsed => usedLeversHash.Count;

    public PlayerModel playerModel;
    public List<GameObject> allCollectablePrefabs;

    private HashSet<LeverController> usedLeversHash = new HashSet<LeverController>();

    public HashSet<string> collectablesCollected { get; private set; } = new HashSet<string>();

    public Dictionary<string, GameObject> collectablePrefabsDict { get; private set; } =
        new Dictionary<string, GameObject>();

    private const string collectablePlayerPrefsKey = "collectables";

    new void Awake()
    {
        base.Awake();
        PrepareCollectableDict();
        LoadCollectables();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnLeverUsed(LeverController leverController)
    {
        usedLeversHash.Add(leverController);
    }

    public void OnCollectiblePickedUp(CollectablePickUp collectable)
    {
        collectablesCollected.Add(collectable.collectableName);
        SaveCollectables();
    }

    private void SaveCollectables()
    {
        PlayerPrefsCollectables playerPrefsCollectables =
            new PlayerPrefsCollectables {collectables = collectablesCollected.ToList()};

        var json = JsonUtility.ToJson(playerPrefsCollectables);
        print(json);
        PlayerPrefs.SetString(collectablePlayerPrefsKey, json);
    }

    private void LoadCollectables()
    {
        collectablesCollected.Clear();

        if (PlayerPrefs.HasKey(collectablePlayerPrefsKey))
        {
            var json = PlayerPrefs.GetString(collectablePlayerPrefsKey);
            print(json);
            var playerPrefsCollectables = JsonUtility.FromJson<PlayerPrefsCollectables>(json);
            foreach (var collectable in playerPrefsCollectables.collectables)
            {
                collectablesCollected.Add(collectable);
            }
        }
    }

    private void PrepareCollectableDict()
    {
        collectablePrefabsDict.Clear();
        foreach (var collectablePrefab in allCollectablePrefabs)
        {
            var collectable = collectablePrefab.GetComponent<CollectablePickUp>();
            collectablePrefabsDict[collectable.collectableName] = collectablePrefab;
        }
    }
}