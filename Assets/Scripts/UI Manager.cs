using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

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
    public GameObject PlayerLifeIndicatorOnGameScreen;
    public GameObject PlayerLifeIndicatorOnIntermediateMenu;
    public TextMeshProUGUI playerLifeCountOnGameScreen;
    public TextMeshProUGUI playerLifeCountOnIntermediateMenu;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI enemyRelatedAnnoncementText;
    public GameObject CustomBaseBlockSwitchButtonWithTimer;
    public GameObject playPauseMenu;
    public Button button_playpauseMenu_Sbb;
    public TextMeshProUGUI playPauseScoreText;

    [Header("GameConclude UI")]
    public GameObject gameoverUI;
    public TextMeshProUGUI gameoverStatusText;
    public TextMeshProUGUI GameOverScreenScoreText;
    public GameObject starPannel;
    public List<GameObject> Stars;
    public GameObject smugEnemy;
    public Button nextLevelButton;




    [Header("Enemy Spawn Height Marker")]
    [SerializeField] GameObject enemySpawnHeightMarker;
    [SerializeField] float maxSpawnHeightMarkerLineWidth = 3f;

    [Header("Scrolling and Panning")]
    [SerializeField] Button camRecentreButton;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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

    public void SetSpawnHeightMarker(int height,List<Vector3> EndPts)
    {
        LineRenderer lineRenderer = enemySpawnHeightMarker.GetComponent<LineRenderer>();

        lineRenderer.startWidth = maxSpawnHeightMarkerLineWidth;
        lineRenderer.endWidth = maxSpawnHeightMarkerLineWidth;
        lineRenderer.alignment = LineAlignment.View;

        Vector3 startingPoint = new Vector3(EndPts[0].x,height,EndPts[0].z);
        Vector3 endPoint = new Vector3(EndPts[1].x, height, EndPts[1].z);

        lineRenderer.SetPosition(0, startingPoint);
        lineRenderer.SetPosition(1, endPoint);

        Debug.Log("started drawing at" + startingPoint + " to " + endPoint);
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

    public void SetEnemyRelatedReminder(string announcement, Color textColor)
    {
        enemyRelatedAnnoncementText.text = announcement;
        enemyRelatedAnnoncementText.color = textColor;
    }

    public void UpdatePlayerLifeCount(int currentLives)
    { 
        playerLifeCountOnGameScreen.text = currentLives.ToString();
        playerLifeCountOnIntermediateMenu.text = currentLives.ToString();
    }


    public void OnSBBButtonClicked()
    {
        timer.isTimerStarted = false;
        timer.ResetTimerValue();
        CustomBaseBlockSwitchButtonWithTimer.SetActive(false);
        playPauseMenu.SetActive(false);

        PlayerLifeIndicatorOnGameScreen.SetActive(true);
        PlayerLifeIndicatorOnIntermediateMenu.SetActive(false);

        LevelManager.Instance.StartCountDownCoroutie();
    }

    public void OnEndPlayerPhaseButtonClicked()
    { 
        playPauseMenu.SetActive(false );
    }

    public void EnableRecentreButton(bool flag)
    { 
        camRecentreButton.gameObject.SetActive(flag);
    }


    public void TogglePlayerLifeIndicatorOnGameScreenAndConverseOnIntermediateMenu(bool isactiveOngameScreen ,bool isActiveOnIntermediateMenu)
    {
        PlayerLifeIndicatorOnGameScreen.SetActive(isactiveOngameScreen);
        PlayerLifeIndicatorOnIntermediateMenu.SetActive(isActiveOnIntermediateMenu);
    }

    public void OnGameOverWithBaseBlocksRemaining()
    {
        playPauseMenu.SetActive(false);

        TogglePlayerLifeIndicatorOnGameScreenAndConverseOnIntermediateMenu(false,false);

        CustomBaseBlockSwitchButtonWithTimer.SetActive(true);
        Debug.Log("sbb should become active");
        timer.isTimerStarted = true;
    }

    public void OnGameOverWithNOBaseBlockRemaining(bool isSwitchingAvailable)
    {
        playPauseMenu.gameObject.SetActive(true) ;
        TogglePlayerLifeIndicatorOnGameScreenAndConverseOnIntermediateMenu(false,true) ;
        button_playpauseMenu_Sbb.gameObject.SetActive(isSwitchingAvailable);
    }

    public void SetStarsOnGameConclude(int starCount)
    { 
        for (int i = 0; i < starCount; i++)
        {
            Stars[i].SetActive(true);
        }
    }

    public void UIOnGameOver(bool wasVictorious,int score,int starCount,bool wasLastLevel)
    {
        Debug.Log("was victorious : " + wasVictorious);    
        playPauseMenu.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);

        TogglePlayerLifeIndicatorOnGameScreenAndConverseOnIntermediateMenu(false, false);

        nextLevelButton.gameObject.SetActive(!wasLastLevel);
        gameoverUI.SetActive(true);

        GameOverScreenScoreText.text = "Score : " + score.ToString();

        if (wasVictorious)
        {
            Debug.Log("Stars are out");
            starPannel.SetActive(true);
            SetStarsOnGameConclude(starCount);
            gameoverStatusText.text = "VICTORY";
        }
        else
        {
            Debug.Log("Smug is me");
            smugEnemy.SetActive(true);
            gameoverStatusText.text = "Game Over";
        }
    }
    
}
