using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    // Expose the other component publicly
    private SceneController _transitionManager;
    private StageCompletion _stageCompletion;
    public static SceneController sceneController
    {
        get { return Instance._transitionManager; }
    }
    public static StageCompletion Stages
    {
        get { return Instance._stageCompletion; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _transitionManager = GetComponent<SceneController>();
        _stageCompletion = GetComponent<StageCompletion>();
    }
}
