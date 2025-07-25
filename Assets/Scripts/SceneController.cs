using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool wasLastLevelInBuildIndex()
    { 
        return(SceneManager.GetActiveScene().buildIndex == (SceneManager.sceneCountInBuildSettings-1));
    }

    public void OnClickedPlay(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void OnClickedExit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }

    public void OnClickedRestart()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnClickedHome()
    {
        SceneManager.LoadScene(0);
    }

    public void OnClickLevelSelect()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnLevelSelectSceneLoaded;

        // Now trigger the load
        SceneManager.LoadScene(0);
    }

    public void OnPlayNextLevelClicked()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnLevelSelectSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe to avoid stacking up calls
        SceneManager.sceneLoaded -= OnLevelSelectSceneLoaded;

        // Get the new scene's UIManager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OnClickedPlayOption();
        }
        else
        {
            Debug.LogWarning("UIManager not found in newly loaded scene.");
        }
    }

}
