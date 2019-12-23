using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObjectiveUI();
    }

    private void UpdateObjectiveUI()
    {
        string objectiveStr = "";

        if (ObjectiveManager.Instance.curObjective != null)
        {
            switch (ObjectiveManager.Instance.curObjective.objectiveType)
            {
                case ObjectiveType.UseLevers:
                    objectiveStr =
                        $"- Use levers to unlock door: {GameManager.Instance.leversUsed}/{ObjectiveManager.Instance.curObjective.intParam}";
                    break;
                case ObjectiveType.GetToDoor:
                    objectiveStr = "- Get to the door";
                    break;
            }
        }

        objectiveText.text = objectiveStr;
    }
}