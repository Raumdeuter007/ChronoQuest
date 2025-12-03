using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class ShowStage : MonoBehaviour
{
    private Dictionary<Stage, bool> stageCompletion;
    public void Start()
    {
        UpdateStageVisibility();
    }
    void UpdateStageVisibility()
    {

        stageCompletion = GameManager.Stages.GetAllStageCompletion();
        stageCompletion[Stage.DarkForest] = true;
        foreach (Stage stage in System.Enum.GetValues(typeof(Stage)))
        {
            // Find child GameObject with matching name
            Transform child = transform.Find(stage.ToString());

            if (child != null)
            {
                // Show only if NOT complete (value is false)
                bool isComplete = stageCompletion.ContainsKey(stage) && stageCompletion[stage];
                child.gameObject.SetActive(!isComplete);
            }
            else
            {
                Debug.LogWarning($"Child GameObject '{stage}' not found!");
            }
        }
    }
}