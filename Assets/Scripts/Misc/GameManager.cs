using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public int leversUsed => usedLeversHash.Count;

    public PlayerModel playerModel;

    private HashSet<LeverController> usedLeversHash = new HashSet<LeverController>();

    new void Awake()
    {
        base.Awake();
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
}
