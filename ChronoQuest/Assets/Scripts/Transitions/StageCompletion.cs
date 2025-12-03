using System.Collections.Generic;
using UnityEngine;

public enum Stage { Snow, Jungle, DarkForest, Dungeon, Grass };
public class StageCompletion : MonoBehaviour
{

    private Dictionary<Stage, bool> stageCompletion = new Dictionary<Stage, bool>();
    void Awake()
    {
        foreach (Stage stage in System.Enum.GetValues(typeof(Stage)))
        {
            stageCompletion[stage] = false;
        }
    }
    public void CompleteStage(Stage stage)
    {
        if (stageCompletion.ContainsKey(stage))
        {
            stageCompletion[stage] = true;
            Debug.Log($"{stage} completed!");
        }
    }
    public bool IsStageComplete(Stage stage)
    {
        return stageCompletion.ContainsKey(stage) && stageCompletion[stage];
    }
    public Dictionary<Stage, bool> GetAllStageCompletion()
    {
        return new Dictionary<Stage, bool>(stageCompletion);
    }
    public void ResetAllProgress()
    {
        foreach (Stage stage in System.Enum.GetValues(typeof(Stage)))
        {
            stageCompletion[stage] = false;
        }
    }
}
