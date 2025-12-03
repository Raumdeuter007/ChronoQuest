using UnityEngine;
using System.Collections.Generic;

public class LevelFinish : MonoBehaviour
{
    [SerializeField] private Stage levelName;
    public void Finish()
    {
        GameManager.Stages.CompleteStage(levelName);
        Dictionary<Stage, bool> dict = GameManager.Stages.GetAllStageCompletion();
        bool stages = dict.ContainsValue(true);
        if (stages)
            GameManager.sceneController.LoadScene("Time Portal");
        else
            GameManager.sceneController.LoadScene("The Reset");
    }
}
