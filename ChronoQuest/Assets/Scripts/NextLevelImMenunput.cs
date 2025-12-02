using UnityEngine;
using UnityEngine.SceneManagement; 

public class NextLevelInput : MonoBehaviour
{
    // This function runs every single frame, BUT only if the object is Active.
    void Update()
    {
        // Check if ANY key (Keyboard, Mouse, Joystick) is pressed
        if (Input.anyKeyDown)
        {
            LoadNextLevel();
        }
    }

    void LoadNextLevel()
    {
        // This gets the number of the current level and adds 1
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check if the next level actually exists in Build Settings
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels! Loading Main Menu (Scene 0).");
            SceneManager.LoadScene(0); 
        }
    }
}