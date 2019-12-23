using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ObjectiveType
{
    UseLevers,
    GetToDoor,
}

[Serializable]
public class Objective
{
    public ObjectiveType objectiveType;
    public int intParam;
    public float floatParam;
    public string stringParam;
}

public class ObjectiveManager : SingletonMonoBehaviour<ObjectiveManager>
{
    public List<Objective> objectives;

    public int curObjectiveIndex { get; private set; } = 0;
    public Objective curObjective => curObjectiveIndex < objectives.Count ? objectives[curObjectiveIndex] : null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObjectiveStatus();
    }

    private void UpdateObjectiveStatus()
    {
        if (curObjectiveIndex >= objectives.Count)
        {
            return;
        }

        Objective curObjective = objectives[curObjectiveIndex];
        switch (curObjective.objectiveType)
        {
            case ObjectiveType.UseLevers:
                if (GameManager.Instance.leversUsed >= curObjective.intParam)
                {
                    OnCurObjectiveCompleted();
                }

                break;
            case ObjectiveType.GetToDoor:
                break;
        }
    }

    private void OnCurObjectiveCompleted()
    {
        curObjectiveIndex++;
    }
}