using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]

    //Credits Panel
    public GameObject creditsPanel; 
    // --- PLAY BUTTON LOGIC 
    public void PlayGame()
    {
        Debug.Log("Play Button Pressed! Loading Level 1...");
        // Loads the next scene in the Build Settings list (Index 1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // --- CREDITS BUTTON LOGIC ---
    public void OpenCredits()
    {
        Debug.Log("Opening Credits...");
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true); // Shows the panel
        }
    }

    public void CloseCredits()
    {
        Debug.Log("Closing Credits...");
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false); // Hides the panel
        }
    }

    // --- QUIT BUTTON LOGIC ---
    public void QuitGame()
    {
        Debug.Log("Quit Button Pressed! Closing Game.");
        Application.Quit(); // Closes the built game

        // This line stops the game in the Unity Editor so you know it worked
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}