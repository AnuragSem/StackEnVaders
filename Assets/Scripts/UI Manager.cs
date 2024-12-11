using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Go Refrences")]
    [SerializeField] Timer timer;
    [SerializeField] StateManager stateManager;

    //homescreenUI
    [Header("Home Screen UI")]
    [SerializeField] GameObject MenuOptionPanel;
    [SerializeField] GameObject LevelSelectPanel;


    //Game UI
    [Header("Game UI element")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countdownText;
    public GameObject CustomBaseBlockSwitchButtonWithTimer;
    public GameObject playPauseMenu;
    public Button button_playpauseMenu_Sbb;
    public TextMeshProUGUI playPauseScoreText;


    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (timer!=null && timer.isTimerStarted)
        {
            Debug.Log("it started continue timer");
            timer.RunSwitchBaseBLockOptionTimer();
        }
    }


    public void DisplayCountdownText(bool state)
    {
        countdownText.gameObject.SetActive(state);
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetPlayPauseUIMenuScore(int score)
    { 
        playPauseScoreText.text = "Score : " +score.ToString();
    }

    public void UpdateCountdownText(string text)
    {
        countdownText.text = text;
    }

    public void OnClickedPlayOption()
    {
        MenuOptionPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        MenuOptionPanel.SetActive(true);
        LevelSelectPanel.SetActive(false);
    }

    void SwitchUIWhenGameOver(bool areBaseBlockRemainging)
    {
        countdownText.gameObject.SetActive(false);
        playPauseMenu.gameObject.SetActive(true);

        playPauseScoreText.gameObject.SetActive(areBaseBlockRemainging);
    }

    public void OnGameOverWithBaseBlocksRemaining()
    {
        playPauseMenu.SetActive(false);
        CustomBaseBlockSwitchButtonWithTimer.SetActive(true);
        Debug.Log("sbb should become active");
        timer.isTimerStarted = true;
        //timer -disable the button if clicked or time suns out
        //+call the other game over ui function with the sbb enabled or disabled depenidng on if base blocks are available
    }

    public void OnGameOverWithNOBaseBlockRemaining(bool isSwitchingAvailable)
    {

        playPauseMenu.gameObject.SetActive(true) ;
        button_playpauseMenu_Sbb.gameObject.SetActive(isSwitchingAvailable);
    }
}
