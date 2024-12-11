using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
